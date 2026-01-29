import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { DashboardWidgetMetric, TopProductResult } from '../../core/models/dashboard_models';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private readonly baseUrl = environment.apiUrl + '/dashboard';

  constructor(private http: HttpClient) {}

  getMetrics(): Observable<readonly DashboardWidgetMetric[]> {
    return this.http
      .get<ApiResponse<readonly DashboardWidgetMetric[]>>(`${this.baseUrl}/metrics`)
      .pipe(map((res) => res.data));
  }

  getTopProducts(limit: number = 5): Observable<readonly TopProductResult[]> {
    return this.http
      .get<ApiResponse<readonly TopProductResult[]>>(`${this.baseUrl}/top-products?limit=${limit}`)
      .pipe(map((res) => res.data));
  }
}
