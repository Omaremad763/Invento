import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
templateUrl:`./paginatation.html`,
styleUrls: ['./paginatation.css']
})
export class PaginationComponent {
  @Input({ required: true }) totalCount: number = 0;
  @Input({ required: true }) pageSize: number = 10;
  @Input({ required: true }) currentPage: number = 1;
  @Output() pageChange = new EventEmitter<number>();

  Math = Math;

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages; i++) {
      pages.push(i);
    }
    return pages;
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.pageChange.emit(page);
    }
  }
}