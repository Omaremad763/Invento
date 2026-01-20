import { Routes } from '@angular/router';
import { DashboardComponent } from './Pages/dashboard/dashboard';
import { ProductManagementComponent } from './Pages/products/products';
export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductManagementComponent },
  //default route
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, 
  //if no route matches, redirect to dashboard
  { path: '**', redirectTo: 'dashboard' } 
];