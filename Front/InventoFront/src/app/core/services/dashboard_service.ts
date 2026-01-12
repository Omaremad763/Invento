import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../..//app/environment';
import { DashboardWidgetMetric, TopProductResult } from '../models/dashboard_models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly baseUrl = environment.apiUrl + '/dashboard';

  constructor(private http: HttpClient) {}

  getMetrics(): Observable<readonly DashboardWidgetMetric[]> {
    return this.http.get<readonly DashboardWidgetMetric[]>(`${this.baseUrl}/metrics`);
  }

  getTopProducts(limit: number = 5): Observable<readonly TopProductResult[]> {
    return this.http.get<readonly TopProductResult[]>(`${this.baseUrl}/top-products?limit=${limit}`);
  }
}