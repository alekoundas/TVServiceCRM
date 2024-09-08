export class UserRegisterRequestDto {
  email: string = "";
  password: string = "";
}

export interface UserRegisterRequestDto {
  [key: string]: any;
  email: string;
  password: string;
}
