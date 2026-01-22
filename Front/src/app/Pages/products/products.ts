import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';
import { Category } from '../../core/models/Category.model';
import { Product } from '../../core/models/Product.model';
import { ProductParams } from '../../core/models/ProductParams-model';
import { CategoryService } from '../../core/services/Category_service';
import { ProductService } from '../../core/services/product-service';
import { environment } from '../../environment';
import { NotificationService } from '../../shared/shared_services/notification.service';

@Component({
  selector: 'app-product-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatPaginator],
  templateUrl: './products.html',
  styleUrls: ['./products.html'],
  animations: [
    trigger('pageAnimations', [
      transition(':enter', [
        query('.header-section, .table-container', [
          style({ opacity: 0, transform: 'translateY(-20px)' }),
          stagger(100, [
            animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
          ]),
        ]),
      ]),
    ]),
    trigger('listAnimation', [
      transition('* => *', [
        query(
          ':enter',
          [
            style({ opacity: 0, transform: 'translateX(-10px)' }),
            stagger(50, [
              animate('300ms ease-out', style({ opacity: 1, transform: 'translateX(0)' })),
            ]),
          ],
          { optional: true },
        ),
      ]),
    ]),
  ],
})
export class ProductManagementComponent implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);
  private NotificationService = inject(NotificationService);
  productParams = new ProductParams();
  totalCount = 0;
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
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
      categoryId: ['', Validators.required],
    });

    this.searchControl.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((val) => {
        const searchTerm = val?.trim() || '';
        this.searchTerm.set(searchTerm);
        if (searchTerm.length > 0 || val === '') {
          this.onSearchTriggered(searchTerm);
        }
      });
  }

  ngOnInit() {
    this.loadProducts();
    this.loadCategories();
  }

  filteredProducts = computed(() => {
    let list = this.products().filter(
      (p) =>
        p.name.toLowerCase().includes(this.searchTerm().toLowerCase()) ||
        p.sku.toLowerCase().includes(this.searchTerm().toLowerCase()),
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
    this.productService.getAllProducts(this.productParams).subscribe({
      next: (response) => {
        if (response) {
          this.products.set(response.data);
          this.productParams.pageNumber = response.pageNumber;
          this.productParams.pageSize = response.pageSize;
          this.totalCount = response.totalCount;
        }
      },
      error: (err) => console.log(err),
    });
  }

  loadCategories() {
    this.categoryService.getAllCategories(this.productParams).subscribe({
      next: (response) => {
        if (response) {
          this.categories.set(response.data);
          this.productParams.pageNumber = response.pageNumber;
          this.productParams.pageSize = response.pageSize;
          this.totalCount = response.totalCount;
        }
      },
      error: (err) => console.log(err),
    });
  }

  toggleSort(key: keyof Product) {
    if (this.sortKey() === key) this.sortDir.update((d) => (d === 'asc' ? 'desc' : 'asc'));
    else {
      this.sortKey.set(key);
      this.sortDir.set('asc');
    }
  }

  deleteProduct(id: string) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will not be able to recover this product!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
      if (result.isConfirmed) {
        this.productService.deleteProduct(id).subscribe({
          next: (res) => {
            if (res.success) {
              Swal.fire({
                text: 'Product Deleted successfully',
                icon: 'success',
                iconColor: '#17DB3E',
              }).then(() => {
                this.loadProducts();
              });
            }
          },
          error: (err) => {
            Swal.fire({
              text: 'Failed to delete product',
              icon: 'error',
              iconColor: 'rgb(246, 22, 41)',
            });
          },
        });
      }
    });
  }
  saveProduct() {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    const productData = this.productForm.value;
    const request$ = this.isEditMode()
      ? this.productService.updateProduct(productData)
      : this.productService.addProduct(productData);
    const actionText = this.isEditMode() ? 'Updated' : 'Added';
    request$.subscribe({
      next: (res) => {
        if (res.success) {
          Swal.fire({
            text: `Product ${actionText} successfully`,
            icon: 'success',
            iconColor: '#17DB3E',
          }).then(() => {
            this.loadProducts();
            this.closeModal();
          });
        }
      },
      error: (err) => {
        console.error(err);
        Swal.fire({
          text: `Failed to ${this.isEditMode() ? 'update' : 'add'} product`,
          icon: 'error',
          iconColor: 'rgb(246, 22, 41)',
        });
      },
    });
  }

  openModal(product?: Product) {
    this.isEditMode.set(!!product);
    if (product) this.productForm.patchValue(product);
    else this.productForm.reset({ price: 0, stockQuantity: 0, categoryId: '' });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  onPageChanged(event: PageEvent) {
    this.productParams.pageNumber = event.pageIndex + 1;

    this.productParams.pageSize = event.pageSize;

    this.loadProducts();
  }

  onCategoryChange(event: Event) {
    console.log('Selected Category ID:', this.productParams.categoryId);
    const selectElement = event.target as HTMLSelectElement;
    const selectedId = selectElement.value;
    this.productParams.categoryId = selectedId === '' ? undefined : selectedId;
    this.currentPage.set(1);
    this.productParams.pageNumber = 1;
    this.loadProducts();
  }

  onSearchTriggered(term: string) {
    this.productParams.pageNumber = 1;
    this.currentPage.set(1);
    this.productParams.searchTerm = term;
    this.loadProducts();
  }
}
