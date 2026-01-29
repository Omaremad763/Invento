import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, NgZone, signal } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import * as AuthDtos from '../shared_models/Auth-models';
import { AuthStateService } from './AuthStateService';

export interface UserState {
  userId: string;
  username: string;
  roles: string[];
}
@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Auth`;
  currentUser = signal<UserState | null>(null);
  private route = inject(Router);
  private zone = inject(NgZone);
  private authState = inject(AuthStateService);
  constructor() {
    this.initializeAuthState();
  }
  private initializeAuthState() {
    const token = this.getToken();
    const storedUser = localStorage.getItem('user_data');
    if (token && storedUser) {
      this.currentUser.set(JSON.parse(storedUser));
    }
  }
  login(data: AuthDtos.LoginDto): Observable<AuthDtos.LoginResponse> {
    return this.http.post<ApiResponse<AuthDtos.LoginResponse>>(`${this.baseUrl}/login`, data).pipe(
      tap((response) => {
        if (response.success && response.data) {
          this.saveToken(response.data.token);
          this.extractAndSaveClaims(response.data.token);
        }
      }),
      map((res) => res.data),
    );
  }
  register(data: AuthDtos.RegisterDto): Observable<AuthDtos.RegisterResponse> {
    return this.http
      .post<ApiResponse<AuthDtos.RegisterResponse>>(`${this.baseUrl}/register`, data)
      .pipe(map((res) => res.data));
  }
  // AuthWithGoogle(idToken: AuthDtos.AuthByGoogleDTO): Observable<AuthDtos.ExternalAuthResponse> {
  //   return this.http
  //     .post<ApiResponse<AuthDtos.ExternalAuthResponse>>(`${this.baseUrl}/GoogleAuth`, idToken)
  //     .pipe(
  //       tap((res) => {
  //         if (res.success && res.data) this.saveToken(res.data.token);
  //       }),
  //       map((res) => res.data),
  //     );
  // }

  AuthWithGithub(code: AuthDtos.ExternalAuthDTO): Observable<AuthDtos.ExternalAuthResponse> {
    return this.http
      .post<ApiResponse<AuthDtos.ExternalAuthResponse>>(`${this.baseUrl}/GithubAuth`, code)
      .pipe(
        tap((res) => {
          if (res.success && res.data) this.saveToken(res.data.token);
        }),
        map((res) => res.data),
      );
  }
  confirmEmail(data: AuthDtos.ConfirmEmailDTO): Observable<AuthDtos.ConfirmResponse> {
    const params = new HttpParams().set('userId', data.userID).set('token', data.token);
    return this.http
      .get<ApiResponse<AuthDtos.ConfirmResponse>>(`${this.baseUrl}/confirm-email`, { params })
      .pipe(map((res) => res.data));
  }
  getToken(): string | null {
    return localStorage.getItem('token');
  }
  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }
  private extractAndSaveClaims(token: string): void {
    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = window.atob(payloadBase64);
      const payload = JSON.parse(decodeURIComponent(escape(payloadJson)));

      const idClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
      const userNameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
      const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

      const rolesRaw = payload[roleClaim];
      const userState: UserState = {
        userId: payload[idClaim],
        username: payload[userNameClaim],
        roles: Array.isArray(rolesRaw) ? rolesRaw : [rolesRaw],
      };
      this.currentUser.set(userState);
      localStorage.setItem('user_data', JSON.stringify(userState));
    } catch (error) {
      console.error('Error decoding token', error);
      this.logout();
    }
  }
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user_data');
    this.currentUser.set(null);
    this.authState.setAuth(false);
    this.route.navigate(['/login']);
  }
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
