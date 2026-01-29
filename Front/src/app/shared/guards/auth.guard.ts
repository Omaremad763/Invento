import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '../shared_services/AuthStateService';

export const authGuard: CanActivateFn = () => {
  const authState = inject(AuthStateService);
  const router = inject(Router);

  let isAuth = false;
  authState.isAuth$.subscribe((v) => (isAuth = v)).unsubscribe();

  if (!isAuth) {
    router.navigate(['/login'], { replaceUrl: true });
    return false;
  }

  return true;
};
