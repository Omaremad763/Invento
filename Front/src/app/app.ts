import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Footer } from './shared/footer/footer';
import { LoadingSpinner } from './shared/loading_spinner/loading-spinner';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoadingSpinner, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  showNavbar = signal(true);
  protected readonly title = signal('InventoFront');
  private router = inject(Router);
}
