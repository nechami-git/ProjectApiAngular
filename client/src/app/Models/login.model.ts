export class LoginModel{
    email!: string;
    password!: string;
}

export interface UserToken {
  id: string;
  email: string;
  role: string;
  name:string;
}