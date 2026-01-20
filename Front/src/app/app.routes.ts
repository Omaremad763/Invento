import { Routes } from '@angular/router';
import { DashboardComponent } from './Pages/dashboard/dashboard';
import { ProductManagementComponent } from './Pages/products/products';

export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductManagementComponent },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, 
  { path: '**', redirectTo: 'dashboard' } 
];