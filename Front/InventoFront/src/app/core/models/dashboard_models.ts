export interface DashboardWidgetMetric {
  readonly name: string;
  readonly value: number;
  readonly unit: string;
}

export interface TopProductResult {
  readonly productId: string;
  readonly productName: string;
  readonly totalSoldQuantity: number;
}