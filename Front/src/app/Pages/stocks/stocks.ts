import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';
import { ProductParams } from '../../core/models/ProductParams-model';
import {
  AddStockTransaction,
  GetStockTransactionDto,
  StockTransactionTypeEnum,
} from '../../core/models/StockTransactionModel';
import { StockService } from '../../core/services/stock_service';

@Component({
  selector: 'app-stock-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './stocks.html',
})
export class StockListComponent implements OnInit {
  private stockService = inject(StockService);

  transactions: GetStockTransactionDto[] = [];
  isModalOpen = false;
  selectTransactionType: number;

  StockTransaction: AddStockTransaction = {
    productId: '',
    quantity: 0,
    transactionType: StockTransactionTypeEnum.Purchase,
  };

  params: ProductParams = {
    pageNumber: 1,
    pageSize: 10,
    searchTerm: '',
    orderBy: 'createdAtDesc',
  };

  private searchSubject = new Subject<string>();
  productLookups: any[] = [];
  selectedProductName: string = '';
  isSearchingProducts = false;

  searchControl = new FormControl('');

  ngOnInit(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe((val) => {
        const term = val?.trim() || '';
        this.params.pageNumber = 1;
        this.params.searchTerm = term;
        this.loadTransactions();
      });

    this.onProductList();
    this.loadTransactions(); // initial load
  }

  loadTransactions() {
    this.stockService.GetAlTransaction(this.params).subscribe({
      next: (res) => (this.transactions = res.data),
      error: (err) => this.showError('Could not load stock data'),
    });
  }

  onSearchChange() {
    this.searchSubject.next(this.params.searchTerm || '');
  }

  submitAdd() {
    this.stockService.AddStockTransaction(this.StockTransaction).subscribe({
      next: (res) => {
        if (res.success == true) {
          this.isModalOpen = false;
          this.showSuccess('Transaction recorded successfully!');
          this.loadTransactions();
          this.resetForm();
        } else {
          this.showError(res.errors?.[0] || 'Transaction failed');
        }
      },
      error: (err) => {
        const apiResponse = err.error;
        if (apiResponse && apiResponse.errors && apiResponse.errors.length > 0) {
          this.showError(apiResponse.errors[0]);
        } else {
          this.showError('Transaction failed');
        }
      },
    });
  }

  confirmDelete(id: string) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'This movement will be permanently reverted!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#0f172a', // slate-900
      cancelButtonColor: '#f1f5f9', // slate-100
      confirmButtonText: 'Yes, delete it!',
      customClass: {
        popup: 'rounded-[2.5rem]',
        confirmButton: 'rounded-2xl px-8 py-4 uppercase font-black text-[10px]',
        cancelButton: 'rounded-2xl px-8 py-4 uppercase font-black text-[10px] text-slate-500',
      },
    }).then((result) => {
      if (result.isConfirmed) {
        this.stockService.DeleteStockTransaction(id).subscribe({
          next: () => {
            this.showSuccess('Record deleted');
            this.loadTransactions();
          },
          error: () => this.showError('Deletion failed'),
        });
      }
    });
  }

  private showSuccess(msg: string) {
    Swal.fire({
      toast: true,
      position: 'top-end',
      icon: 'success',
      title: msg,
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      customClass: { popup: 'rounded-2xl font-bold' },
    });
  }

  private showError(msg: string) {
    Swal.fire({
      icon: 'error',
      title: 'Oops...',
      text: msg,
      confirmButtonColor: '#3b82f6',
      customClass: { popup: 'rounded-[2rem]' },
    });
  }

  private resetForm() {
    this.StockTransaction = {
      productId: '',
      quantity: 0,
      transactionType: StockTransactionTypeEnum.Purchase,
    };
  }

  onProductList() {
    this.stockService.getProductsLookup().subscribe((res) => {
      this.productLookups = res.data;
      this.isSearchingProducts = false;
    });
  }

  selectProduct(product: any) {
    this.StockTransaction.productId = product.id;
    this.selectedProductName = product.name;
    this.selectTransactionType = Number(this.StockTransaction.transactionType);
    this.productLookups = [];
  }
}
