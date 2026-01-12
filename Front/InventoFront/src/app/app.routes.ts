import { Routes } from '@angular/router';
import { DashboardComponent } from './/shared/components/dashboard/dashboard';

export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, // تحويل تلقائي للداشبورد
  { path: '**', redirectTo: 'dashboard' } // لو كتب أي مسار غلط
];