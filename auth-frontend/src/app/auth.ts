import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse } from './models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:7002/api/UserLogin'; // Adjust port as needed
  private tokenKey = 'access_token';

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    console.log('Making login request to:', `${this.apiUrl}/login`);
    console.log('Request payload:', credentials);
    
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          console.log('Login response received:', response);
          this.storeToken(response.token);
        })
      );
  }

  private storeToken(token: string): void {
    console.log('Storing token:', token ? 'Token received' : 'No token');
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);
    console.log('Getting token from storage:', token ? 'Token exists' : 'No token');
    return token;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    console.log('Logging out - clearing token');
    localStorage.removeItem(this.tokenKey);
  }

  // Method to get headers with token for API calls
  getAuthHeaders(): { [key: string]: string } {
    const token = this.getToken();
    return token ? { 'Authorization': `Bearer ${token}` } : {};
  }
}
