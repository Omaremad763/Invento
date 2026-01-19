import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../..//app/environment';
import { ApiResponse } from '../models/api-response.model';
import { DashboardWidgetMetric, TopProductResult } from '../models/dashboard_models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly baseUrl = environment.apiUrl + '/dashboard';

  constructor(private http: HttpClient) {}

  getMetrics(): Observable<ApiResponse<readonly DashboardWidgetMetric[]>> {
    return this.http.get<ApiResponse<readonly DashboardWidgetMetric[]>>(`${this.baseUrl}/metrics`);
  }

  getTopProducts(limit: number = 5): Observable<ApiResponse<readonly TopProductResult[]>> {
    return this.http.get<ApiResponse<readonly TopProductResult[]>>(`${this.baseUrl}/top-products?limit=${limit}`);
  }
}