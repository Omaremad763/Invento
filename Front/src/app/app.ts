import { Component, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { MainLayoutComponent } from './shared/Navbar/Nav-bar';
import { Footer } from './shared/footer/footer';
import { LoadingSpinner } from './shared/loading_spinner/loading-spinner';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoadingSpinner, MainLayoutComponent, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  showNavbar = signal(true);
  protected readonly title = signal('InventoFront');
  private router = inject(Router);
  constructor() {
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      const route = this.router.routerState.root.firstChild;
      this.showNavbar.set(!route?.snapshot.data?.['hideNavBar']);
    });
  }
}
