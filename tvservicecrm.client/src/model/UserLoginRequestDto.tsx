export interface UserLoginRequestDto {
  email: string;
  password: string;
}

export class UserLoginRequestDto {
  email: string = "";
  password: string = "";
}
