import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  user: User;
  @Input() appHasRole: string[]
  constructor(private viewContainerRef: ViewContainerRef, private templateref: TemplateRef<any>, private accountService: AccountService) {

    this.accountService.currenUser$.pipe(take(1)).subscribe(user => {
      this.user = user
    })
  }
  ngOnInit(): void {
    if (!this.user?.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }


    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateref);
    }
    else{
      this.viewContainerRef.clear();
    }

  }

}
