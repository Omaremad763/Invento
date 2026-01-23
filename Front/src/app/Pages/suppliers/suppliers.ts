import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import Swal from 'sweetalert2';
import { ProductParams } from '../../core/models/ProductParams-model';
import { SupplierDTO } from '../../core/models/Supplier.rmodel';
import { SupplierService } from '../../core/services/Supplier_service';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  templateUrl: './suppliers.html',
  imports: [CommonModule, ReactiveFormsModule, MatPaginator],
})
export class SuppliersComponent implements OnInit {
  private supplierService = inject(SupplierService);
  private fb = inject(FormBuilder);

  suppliers = signal<SupplierDTO[]>([]);
  totalCount = signal<number>(0);
  loading = signal<boolean>(false);
  showModal = signal<boolean>(false);
  isEditMode = signal<boolean>(false);

  params = signal<ProductParams>({
    pageNumber: 1,
    pageSize: 5,
    searchTerm: '',
    orderBy: 'name',
  });

  supplierForm: FormGroup;

  euCountries = [
    { code: 'DE', name: 'Germany' },
    { code: 'FR', name: 'France' },
    { code: 'IT', name: 'Italy' },
    { code: 'ES', name: 'Spain' },
    { code: 'IE', name: 'Ireland' },
  ];

  constructor() {
    this.supplierForm = this.fb.group({
      id: [''],
      name: ['', [Validators.required, Validators.minLength(3)]],
      contactEmail: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      countryCode: ['', Validators.required],
      vatNumber: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.loadSuppliers();
  }

  loadSuppliers() {
    this.loading.set(true);
    this.supplierService.GetAlSuppliers(this.params()).subscribe({
      next: (res) => {
        this.suppliers.set(res.data);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onPageChange(page: number) {
    this.params.update((p) => ({ ...p, pageNumber: page }));
    this.loadSuppliers();
  }

  onSearch(event: any) {
    const term = event.target.value;
    this.params.update((p) => ({ ...p, searchTerm: term, pageNumber: 1 }));
    this.loadSuppliers();
  }
  saveSupplier() {
    if (this.supplierForm.invalid) return;
    if (this.isEditMode() && this.supplierForm.pristine) {
      Swal.fire('Info', 'No changes detected to update.', 'info');
      return;
    }
    const { countryCode, vatNumber, ...dto } = this.supplierForm.value;
    const request$ = this.isEditMode()
      ? this.supplierService.UpdateSupplier(this.supplierForm.value)
      : this.supplierService.AddSupplier(dto, countryCode, vatNumber);
    request$.subscribe({
      next: (res) => {
        if (res.success) {
          Swal.fire({
            title: 'Success!',
            text: `Supplier ${this.isEditMode() ? 'updated' : 'added'} successfully`,
            icon: 'success',
            confirmButtonText: 'OK',
            confirmButtonColor: '#rgb(4, 179, 33)',
          }).then((result) => {
            if (result.isConfirmed) {
              this.loadSuppliers();
              this.closeModal();
            }
          });
        }
      },
      error: (err) => {
        const errorMessage = err.error?.message || 'Something went wrong';
        Swal.fire('Error', errorMessage, 'error');
      },
    });
  }
  openModal(supplier?: SupplierDTO) {
    const vatControl = this.supplierForm.get('vatNumber');
    const countryControl = this.supplierForm.get('countryCode');
    this.isEditMode.set(!!supplier);
    if (supplier) {
      vatControl?.disable();
      countryControl?.disable();
      this.supplierForm.patchValue(supplier);
    } else {
      this.supplierForm.reset({ countryCode: '', vatNumber: '' });
    }
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  deleteSupplier(id: string) {
    Swal.fire({
      title: 'Are you sure?',
      text: "Supplier will be deleted immediately, but we'll rollback if server fails.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
      if (!result.isConfirmed) return;
      this.supplierService.DeleteSupplier(id).subscribe({
        next: (res) => {
          if (res.success) {
            Swal.fire('Deleted!', 'Supplier deleted successfully', 'success').then(() =>
              this.loadSuppliers(),
            );
          }
        },
        error: () => Swal.fire('Error', 'Server error while deleting', 'error'),
      });
    });
  }
}
