export class UserLoginDto {
  email: string = "";
  password: string = "";
}

export interface UserLoginDto {
  [key: string]: any;
  email: string;
  password: string;
}
