import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from "@angular/router";
import { Observable } from 'rxjs';
import { DashboardWidgetMetric, TopProductResult } from '../../../core/models/dashboard_models';
import { DashboardService } from '../../../core/services/dashboard_service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  imports: [CommonModule, RouterModule]
})
export class DashboardComponent implements OnInit {
  metrics$!: Observable<readonly DashboardWidgetMetric[]>;
  topProducts$!: Observable<readonly TopProductResult[]>;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.metrics$ = this.dashboardService.getMetrics();
    this.topProducts$ = this.dashboardService.getTopProducts(5);
  }
}