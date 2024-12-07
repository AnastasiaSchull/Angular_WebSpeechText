import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service'; 
import { LoginModel } from '../models/login.model';  
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  model = { login: '', password: '' };
  constructor(private authService: AuthService, private router: Router) { } // инжектируем

  login(): void {
    this.authService.login(this.model).subscribe({
      next: (res: any) => {
        localStorage.setItem('token', res.Token);
        this.router.navigate(['/audio']); // перенаправляем на страницу аудио
      },
      error: (err) => {
        console.error('Login error', err);
        alert('Login failed: ' + (err.error || 'Unknown error'));
      }
    });
  }
}

