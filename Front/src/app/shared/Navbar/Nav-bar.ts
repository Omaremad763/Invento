import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MENU_ITEMS } from './NavBarSections';

@Component({
  selector: 'app-Navbar',
  standalone: true, 
  templateUrl: './Nav-bar.html',
  styleUrls: ['./Nav-bar.css'], 
  imports: [RouterModule, CommonModule],
})
export class MainLayoutComponent {
  menuItems = MENU_ITEMS; 
  isSidebarOpen = true; 

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}