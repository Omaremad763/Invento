import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainLayoutComponent } from "./shared/Navbar/Nav-bar";
import { LoadingSpinner } from './shared/loading_spinner/loading-spinner';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LoadingSpinner, MainLayoutComponent],
templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('InventoFront');
}
