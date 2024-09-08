export class UserRegisterDto {
  email: string = "";
  password: string = "";
}

export interface UserRegisterDto {
  [key: string]: any;
  email: string;
  password: string;
}
