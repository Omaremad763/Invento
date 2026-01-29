import { Component, NgZone, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Footer } from './shared/footer/footer';
import { LoadingSpinner } from './shared/loading_spinner/loading-spinner';
import { AuthStateService } from './shared/shared_services/AuthStateService';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoadingSpinner, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  constructor(
    private authState: AuthStateService,
    private router: Router,
    private zone: NgZone,
  ) {
    window.addEventListener('storage', (event) => {
      if (event.key === 'token' && event.newValue === null) {
        this.zone.run(() => {
          this.router.navigate(['/login'], { replaceUrl: true });
        });
      }
    });
  }
  showNavbar = signal(true);
  protected readonly title = signal('InventoFront');
}
