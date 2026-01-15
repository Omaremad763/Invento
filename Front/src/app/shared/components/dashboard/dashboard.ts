import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from "@angular/router";
import { Chart, registerables } from 'chart.js';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import { Observable } from 'rxjs';
import { DashboardWidgetMetric, TopProductResult } from '../../../core/models/dashboard_models';
import { DashboardService } from '../../../core/services/dashboard_service';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  standalone: true, 
  imports: [CommonModule, RouterModule]
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  metrics$!: Observable<readonly DashboardWidgetMetric[]>;
  topProducts$!: Observable<readonly TopProductResult[]>;
  isLoading = false;
  chart: any;
  
  
  liveChart: any; 
  private updateInterval: any;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.metrics$ = this.dashboardService.getMetrics();
    this.topProducts$ = this.dashboardService.getTopProducts(5);
  }

  
  ngAfterViewInit(): void {
    this.initChart();
    this.initActivityChart();
  }

  private initChart(): void {
    const ctx = document.getElementById('mainDashboardChart') as HTMLCanvasElement;
    if (!ctx) return;

    this.chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
        datasets: [{
          label: 'Inventory Movement',
          data: [10, 25, 15, 40, 30, 55, 70],
          borderColor: '#1F4A94',
          backgroundColor: '#B9CFF5',
          fill: true,
          tension: 0.4,
          pointRadius: 4,
          pointBackgroundColor: '#F63B6D'
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: { display: false },
          x: { grid: { display: false } }
        }
      }
    });
  }
private initActivityChart(): void {
  const ctx = document.getElementById('activityChart') as HTMLCanvasElement;
  if (!ctx) return;

  this.liveChart = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: Array(15).fill(''), 
      datasets: [{
        data: Array(15).fill(0).map(() => Math.floor(Math.random() * 40) + 10),
        backgroundColor: '#1F4A94', 
        borderRadius: 4,
        barThickness: 8
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      
      animations: {
        y: {
          duration: 1000, 
          easing: 'easeInOutQuart'
        }
      },
      plugins: { legend: { display: false } },
      scales: {
        x: { display: false },
        y: { 
          display: false, 
          beginAtZero: true,
          suggestedMax: 60 
        }
      }
    }
  });

  this.updateInterval = setInterval(() => {
    if (this.liveChart) {
      const newData = Math.floor(Math.random() * 50) + 5;
      
      
      this.liveChart.data.datasets[0].data.shift();
      this.liveChart.data.datasets[0].data.push(newData);
      
      
      
      this.liveChart.update(); 
    }
  }, 1000); 
}

  public async downloadPDF() {
    const data = document.getElementById('dashboard-content');
    if (!data) return;

    try {
      const canvas = await html2canvas(data, { scale: 2 });
      const imgData = canvas.toDataURL('image/png');
      const imgWidth = canvas.width;
      const imgHeight = canvas.height;

      const pdf = new jsPDF({
        orientation: imgWidth > imgHeight ? 'l' : 'p',
        unit: 'px',
        format: [imgWidth, imgHeight]
      });

      pdf.addImage(imgData, 'PNG', 0, 0, imgWidth, imgHeight);
      pdf.save('Invento-Report.pdf');
    } catch (error) {
      console.error('PDF Generation Error:', error);
    }
  }
  ngOnDestroy(): void {
    if (this.updateInterval) {
      clearInterval(this.updateInterval);
    }
  }
}