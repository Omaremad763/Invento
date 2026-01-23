import { Routes } from '@angular/router';
import { CategoriesPage } from './Pages/categories/categories';
import { DashboardComponent } from './Pages/dashboard/dashboard';
import { ProductManagementComponent } from './Pages/products/products';
import { SuppliersComponent } from './Pages/suppliers/suppliers';

export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductManagementComponent },
  //default route
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'categories', component: CategoriesPage },
  { path: 'suppliers', component: SuppliersComponent },
  //if no route matches, redirect to dashboard must be the last route
  { path: '**', redirectTo: 'dashboard' },
];
