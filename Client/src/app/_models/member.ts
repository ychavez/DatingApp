import { Photo } from "./photo";

export interface Member {
  id: number;
  userName: string;
  age: number;
  photoUrl: string;
  dateIfBirth: string;
  knownAs: string;
  created: string;
  lastActive: Date;
  gender: string;
  introduction: string;
  interests: string;
  city: string;
  country: string;
  lookingFor: string;
  photos: Photo[];
}

