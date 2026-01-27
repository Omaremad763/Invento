import { Routes } from '@angular/router';
import { ConfirmEmailComponent } from './Pages/Auth/ConfirmEmail/ConfirmEmail';
import { LoginComponent } from './Pages/Auth/login/login';
import { RegisterComponent } from './Pages/Auth/register/register';
import { CategoriesPage } from './Pages/categories/categories';
import { DashboardComponent } from './Pages/dashboard/dashboard';
import { ProductManagementComponent } from './Pages/products/products';
import { SuppliersComponent } from './Pages/suppliers/suppliers';

export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductManagementComponent },
  //default route
  { path: '', redirectTo: 'register', pathMatch: 'full' },
  { path: 'categories', component: CategoriesPage },
  { path: 'suppliers', component: SuppliersComponent },
  { path: 'login', component: LoginComponent, data: { hideNavBar: true } },
  { path: 'register', component: RegisterComponent, data: { hideNavBar: true } },
  //if no route matches, redirect to register page must be the selected route
  { path: 'confirm-email', component: ConfirmEmailComponent },
  { path: '**', redirectTo: 'register' },
];
