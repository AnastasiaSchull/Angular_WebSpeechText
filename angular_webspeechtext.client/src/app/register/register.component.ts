import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { RegisterModel } from '../models/register.model';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  model: RegisterModel = new RegisterModel();

  constructor(private authService: AuthService, private router: Router) { }

  register(registerForm: NgForm) {
    if (registerForm.valid) {
      this.authService.register(this.model).subscribe({
        next: (res) => {
          console.log('Registration successful', res);
          this.router.navigate(['/audio']);
        },
        error: (err) => {
          console.error('Registration failed', err);
        }
      });
    }
  }
}
