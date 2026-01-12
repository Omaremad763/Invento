import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MENU_ITEMS } from '../../core/constants/menu';


@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  imports: [RouterModule, CommonModule],
})
export class MainLayoutComponent {
  menuItems = MENU_ITEMS; // سحب المصفوفة عشان نعرضها في الـ HTML
  isSidebarOpen = true;   // للتحكم في غلق وفتح المنيو

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}