import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AppInterceptor } from './shared/interceptors/global_interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimationsAsync(),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
    withInterceptorsFromDi() 
  ),
    { provide: HTTP_INTERCEPTORS, useClass: AppInterceptor, multi: true },
  ]
};
