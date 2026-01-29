import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private _isAuth$ = new BehaviorSubject<boolean>(this.hasToken());

  isAuth$ = this._isAuth$.asObservable();

  setAuth(value: boolean) {
    this._isAuth$.next(value);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }
}
