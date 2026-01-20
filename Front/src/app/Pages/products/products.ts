import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { Product } from '../../core/models/Product.model';
import { ApiResponse } from '../../core/models/api-response.model';
import { environment } from '../../environment';
import { PaginationComponent } from '../../shared/paginatation/paginatation';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  templateUrl: './products.html',
  styleUrls: ['./products.html'], 
  animations: [
    trigger('pageAnimations', [
      transition(':enter', [
        query('.header-section, .table-container', [
          style({ opacity: 0, transform: 'translateY(-20px)' }),
          stagger(100, [animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))])
        ])
      ])
    ]),
    trigger('listAnimation', [
      transition('* => *', [
        query(':enter', [
          style({ opacity: 0, transform: 'translateX(-10px)' }),
          stagger(50, [animate('300ms ease-out', style({ opacity: 1, transform: 'translateX(0)' }))])
        ], { optional: true })
      ])
    ])
  ]
})
export class ProductManagementComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  products = signal<Product[]>([]);
  categories = signal<any[]>([]); 
  searchTerm = signal<string>('');
  sortKey = signal<keyof Product>('name');
  sortDir = signal<'asc' | 'desc'>('asc');
  currentPage = signal<number>(1);
  pageSize = 5;
  showModal = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  private readonly apiUrl = `${environment.apiUrl}/Products`;
  searchControl = new FormControl('');
  productForm: FormGroup;

  constructor() {
    this.productForm = this.fb.group({
      id: [''],
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0.1)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: ['', Validators.required]
    });

    
    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntilDestroyed()
    ).subscribe(val => {
      this.searchTerm.set(val || '');
      this.currentPage.set(1);
    });
  }

  ngOnInit() {
    this.loadProducts();
    // this.loadCategories(); 
  }

  
  filteredProducts = computed(() => {
    let list = this.products().filter(p => 
      p.name.toLowerCase().includes(this.searchTerm().toLowerCase()) || 
      p.sku.toLowerCase().includes(this.searchTerm().toLowerCase())
    );

    return list.sort((a, b) => {
      const valA = a[this.sortKey()];
      const valB = b[this.sortKey()];
      return (valA < valB ? -1 : 1) * (this.sortDir() === 'asc' ? 1 : -1);
    });
  });

  paginatedProducts = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.filteredProducts().slice(start, start + this.pageSize);
  });

  totalPages = computed(() => Math.ceil(this.filteredProducts().length / this.pageSize));

  
  loadProducts() {
    this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/GetAlProducts`)
      .subscribe(res => { if(res.success) this.products.set(res.data); });
  }

  loadCategories() {
    this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/GetAllCategories`)
      .subscribe(res => { if(res.success) this.categories.set(res.data); });
  }

  toggleSort(key: keyof Product) {
    if (this.sortKey() === key) this.sortDir.update(d => d === 'asc' ? 'desc' : 'asc');
    else { this.sortKey.set(key); this.sortDir.set('asc'); }
  }

  deleteProduct(id: string) {
    
    const oldProducts = this.products();
    this.products.set(oldProducts.filter(p => p.id !== id));

    this.http.delete(`${this.apiUrl}/Delete/${id}`).subscribe({
      error: () => { this.products.set(oldProducts); alert('Error deleting product'); }
    });
  }

  saveProduct() {
    if (this.productForm.invalid) return;
    const dto = { product: this.productForm.value };

    if (this.isEditMode()) {
      this.http.put<ApiResponse<any>>(`${this.apiUrl}/UpdateProduct`, dto).subscribe(res => {
        if(res.success) { this.loadProducts(); this.closeModal(); }
      });
    } else {
      this.http.post<ApiResponse<any>>(`${this.apiUrl}/AddProduct`, dto).subscribe(res => {
        if(res.success) { this.loadProducts(); this.closeModal(); }
      });
    }
  }

  openModal(product?: Product) {
    this.isEditMode.set(!!product);
    if (product) this.productForm.patchValue(product);
    else this.productForm.reset({ price: 0, stockQuantity: 0, categoryId: '' });
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

onPageChange(newPage: number) {
  this.currentPage.set(newPage);
}

}