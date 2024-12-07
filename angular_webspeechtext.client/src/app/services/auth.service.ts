import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterModel } from '../models/register.model'; 
import { LoginModel } from '../models/login.model'; 
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5048/api/account';
  constructor(private http: HttpClient, private router: Router) { }

  register(model: RegisterModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, model);
  }

  login(model: LoginModel): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, model).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token); // сохраняем токен в localStorage
          this.router.navigate(['/audio']); // перенаправляем на страницу аудио
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token'); //для проверки токена в localStorage
  }

}
