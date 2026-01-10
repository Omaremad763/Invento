import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class AppInterceptor implements HttpInterceptor {
  
  constructor(
    private loadingService: LoadingService,
    private notification: NotificationService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // 1. أظهر الـ Spinner أول ما الـ Request يبدأ
    this.loadingService.show();

    // 2. إضافة الـ Token (لو موجود)
    const token = localStorage.getItem('token');
    if (token) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(request).pipe(
      // 3. التعامل مع الأخطاء بشكل عالمي (Global Error Handling)
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unknown error occurred!';
        if (error.status === 401) errorMessage = 'Session expired. Please login again.';
        if (error.status === 403) errorMessage = 'You do not have permission to do this.';
        
        // استخدم SweetAlert لإظهار الخطأ
        this.notification.showError(errorMessage); // افترضنا عندك ميثود للخطأ في الـ service
        return throwError(() => error);
      }),
      // 4. إخفاء الـ Spinner لما الـ Request يخلص (سواء نجح أو فشل)
      finalize(() => {
        this.loadingService.hide();
      })
    );
  }
}