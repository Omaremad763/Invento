import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'metric-card',
  standalone: true,
  templateUrl: './metrics.html',
  styleUrls: ['./metrics.css'],
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardMetricCardComponent implements OnInit {

  @Input({ required: true }) title!: string;
  @Input({ required: true }) value!: number;
  @Input() unit: string = '';
  @Input() icon: string = '';

  displayValue = 0;

  ngOnInit(): void {
    this.animateCount();
  }

  private animateCount(): void {
    if (this.value <= 0) {
      this.displayValue = this.value;
      return;
    }

    const duration = 500;
    const startTime = performance.now();

    const step = (currentTime: number) => {
      const progress = Math.min((currentTime - startTime) / duration, 1);
      this.displayValue = Math.floor(progress * this.value);

      if (progress < 1) {
        requestAnimationFrame(step);
      }
    };

    requestAnimationFrame(step);
  }
}
