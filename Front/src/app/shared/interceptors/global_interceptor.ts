import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { ApiResponse } from '../shared_models/api-response.model';
import { AuthService } from '../shared_services/auth.service';
import { LoadingService } from '../shared_services/loading.service';
import { NotificationService } from '../shared_services/notification.service';

@Injectable()
export class AppInterceptor implements HttpInterceptor {
  // استخدام inject للخدمات الجديدة
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor(
    private loadingService: LoadingService,
    private notification: NotificationService,
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.loadingService.show();
    const token = this.authService.getToken();

    if (token) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    }

    return next.handle(request).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          const body = event.body as ApiResponse<any>;
          if (body && body.hasOwnProperty('success')) {
            if (body.success) {
              return event;
            } else {
              const errorMessage = body.errors?.join(', ') || 'Operation failed';
              this.notification.showError(errorMessage);
              throw new Error(errorMessage);
            }
          }
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unknown error occurred!';

        if (error.status === 401) {
          // 2. استخدام دالة logout اللي عملناها في السيرفس عشان ننظف الـ Signals والـ Storage
          Swal.fire({
            title: 'Unauthorized!',
            text: 'Session expired. Please login again.',
            icon: 'error',
            confirmButtonText: 'OK',
          }).then(() => {
            this.authService.logout(); // تنظيف الحالة
            this.router.navigate(['/login']);
          });
        }

        if (error.status === 403) {
          errorMessage = 'You do not have permission to do this.';
        } else if (error.error && error.error.errors) {
          errorMessage = error.error.errors.join(', ');
        } else {
          errorMessage = error.message;
        }

        this.notification.showError(errorMessage);
        return throwError(() => error);
      }),
      finalize(() => {
        this.loadingService.hide();
      }),
    );
  }
}
