import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AppInterceptor } from './core/interceptors/global_interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
    withInterceptorsFromDi() 
  ),
    { provide: HTTP_INTERCEPTORS, useClass: AppInterceptor, multi: true },
  ]
};
