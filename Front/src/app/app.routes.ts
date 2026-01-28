import { Routes } from '@angular/router';
import { LoginComponent } from './Pages/Auth/login/login';
import { RegisterComponent } from './Pages/Auth/register/register';
import { CategoriesPage } from './Pages/categories/categories';
import { DashboardComponent } from './Pages/dashboard/dashboard';
import { ProductManagementComponent } from './Pages/products/products';
import { SuppliersComponent } from './Pages/suppliers/suppliers';
import { AdminLayoutComponent } from './shared/Helper Componnets/AdminLayoutComponent';
import { AuthLayoutComponent } from './shared/Helper Componnets/AuthLayoutComponent';
//layered pattern
export const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' },
    ],
  },
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      // { path: 'purchases', component: PurchasesComponent },
      { path: 'products', component: ProductManagementComponent },
      { path: 'categories', component: CategoriesPage },
      { path: 'suppliers', component: SuppliersComponent },
    ],
  },
];
