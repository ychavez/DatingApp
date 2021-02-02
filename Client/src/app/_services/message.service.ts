import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/messages';
import { User } from '../_models/user';
import { getPaginatedResult, paginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messagesThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messagesThreadSource.asObservable();

  constructor(private http: HttpClient) { }
  
  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + '/messages?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on("ReceiveMessageThread", messages =>{

      this.messagesThreadSource.next(messages);
   
    } )
    this.hubConnection.on('NewMessage',message =>{
      this.messageThread$.pipe(take(1)).subscribe(messages =>{
        this.messagesThreadSource.next([...messages,message])
      })
    })
  }

  stopConnection() {
    if(this.hubConnection){
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber, pageSize, container) {
    let params = paginationHeaders(pageNumber, pageSize);
    params = params.append('container', container);
    return getPaginatedResult<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.http
    );
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }
  async sendMessage(username: string, content: string) {
    return this.hubConnection.invoke('sendMessage', {
      recipientUserName: username,
      content,
    }).catch(error =>console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id)
  }
}
