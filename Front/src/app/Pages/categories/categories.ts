import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';
import { Category } from '../../core/models/Category.model';
import { ProductParams } from '../../core/models/ProductParams-model';
import { CategoryService } from '../../core/services/Category_service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.html',
})
export class CategoriesPage implements OnInit {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);

  searchControl = new FormControl('');
  searchTerm = signal<string>('');

  params = signal<ProductParams>({
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    orderBy: 'name',
  });

  private initSearchDebounce() {
    this.searchControl.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((val) => {
        const term = val?.trim() || '';
        this.onSearchTriggered(term);
      });
  }

  onSearchTriggered(term: string) {
    this.searchTerm.set(term);

    this.params.update((prev) => ({
      ...prev,
      searchTerm: term,
      pageNumber: 1,
    }));

    this.loadCategories();
  }

  showModal = signal<boolean>(false);
  categories = signal<Category[]>([]);
  totalCount = signal(0);
  isLoading = signal(false);
  isEditMode = signal(false);

  selectedCategoryId = signal<string | null>(null);

  categoryForm: FormGroup = this.fb.group({
    CategoryName: ['', [Validators.required, Validators.minLength(3)]],
  });

  constructor() {
    this.initSearchDebounce();
  }

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.isLoading.set(true);

    this.categoryService.getAllCategories(this.params()).subscribe({
      next: (res) => {
        this.categories.set(res.data);
        this.totalCount.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }

  onSave() {
    if (this.categoryForm.invalid) return;

    const request$ = this.isEditMode()
      ? this.categoryService.updateCategory({
          ...this.categoryForm.value,
          id: this.selectedCategoryId(),
        })
      : this.categoryService.addCategory(this.categoryForm.value.CategoryName);

    request$.subscribe((res) => {
      if (res.success) {
        this.loadCategories();
        this.closeModal();
        Swal.fire('Success', `Category ${this.isEditMode() ? 'updated' : 'added'}`, 'success');
      }
    });
  }

  onDelete(id: string) {
    Swal.fire({
      title: 'Are you sure?',
      text: "Category will be deleted immediately, but we'll rollback if server fails.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
      if (!result.isConfirmed) return;
      this.categoryService.deleteCategory(id).subscribe({
        next: (res) => {
          if (res.success) {
            Swal.fire('Deleted!', 'Category deleted successfully', 'success').then(() =>
              this.loadCategories(),
            );
          }
        },
        error: () => Swal.fire('Error', 'Server error while deleting', 'error'),
      });
    });
  }

  openModal(category?: Category) {
    if (category) {
      this.isEditMode.set(true);
      this.selectedCategoryId.set(category.id);
      this.categoryForm.patchValue({
        CategoryName: category.categoryName,
      });
    } else {
      this.isEditMode.set(false);
      this.categoryForm.reset();
    }
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedCategoryId.set(null);
    this.categoryForm.reset();
  }
}
