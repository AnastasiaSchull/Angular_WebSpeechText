export class RegisterModel {
  constructor(
    public firstName: string = '',
    public lastName: string = '',
    public login: string = '',
    public password: string = '',
    public passwordConfirm: string = ''
  ) { }
}

