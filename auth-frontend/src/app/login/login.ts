import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth';
import { LoginRequest } from '../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  credentials: LoginRequest = {
    username: '',
    password: ''
  };
  
  successMessageShow = false;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  storedToken = '';

  constructor(private authService: AuthService) {
    this.loadStoredToken();
  }

  onLogin(): void {
    console.log('Login attempt with:', this.credentials);
    this.isLoading = true;
    this.successMessageShow = false;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        this.successMessage = 'Login successful! Token stored.';
        this.successMessageShow = true;        
        this.loadStoredToken();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Login failed:', error);
        this.errorMessage = error.error || 'Login failed. Please try again.';
        this.isLoading = false;
      }
    });
  }

  private loadStoredToken(): void {
    this.storedToken = this.authService.getToken() || '';
    console.log('Loaded stored token:', this.storedToken ? 'Token exists' : 'No token');
  }

  logout(): void {
    this.authService.logout();
    this.storedToken = '';
    this.successMessage = 'Logged out successfully.';
    this.successMessageShow = true;
  }
}
