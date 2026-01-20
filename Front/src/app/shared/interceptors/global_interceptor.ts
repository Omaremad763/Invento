import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';
import { ApiResponse } from '../../core/models/api-response.model';
import { LoadingService } from '../../core/services/loading.service';
import { NotificationService } from '../../core/services/notification.service';

@Injectable()
export class AppInterceptor implements HttpInterceptor {
  
  constructor(
    private loadingService: LoadingService,
    private notification: NotificationService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    
    this.loadingService.show();

    
    const token = localStorage.getItem('token');
    if (token) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(request).pipe(
      
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          const body = event.body as ApiResponse<any>;
          
          
          if (body && body.hasOwnProperty('success')) {
            if (body.success) {
              
              return event.clone({ body: body.data });
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
        
        
        if (error.error && error.error.errors) {
            errorMessage = error.error.errors.join(', ');
        } else if (error.status === 401) {
            errorMessage = 'Session expired. Please login again.';
        } else if (error.status === 403) {
            errorMessage = 'You do not have permission to do this.';
        } else {
            errorMessage = error.message;
        }

        this.notification.showError(errorMessage); 
        return throwError(() => error);
      }),
      
      finalize(() => {
        this.loadingService.hide();
      })
    );
  }
}