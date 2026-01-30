import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import { PaginatedResponse } from '../../shared/shared_models/PaginatedResponse';
import { ProductParams } from '../models/ProductParams-model';
import {
  AddStockTransaction,
  GetProductsLookUpDTO,
  GetStockTransactionDto,
} from '../models/StockTransactionModel';
@Injectable({ providedIn: 'root' })
export class StockService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/StockTransaction`;

  GetAlTransaction(
    productParams: ProductParams,
  ): Observable<PaginatedResponse<GetStockTransactionDto>> {
    let params = new HttpParams();

    if (productParams.pageNumber) {
      params = params.append('pageNumber', productParams.pageNumber.toString());
    }

    if (productParams.pageSize) {
      params = params.append('pageSize', productParams.pageSize.toString());
    }

    if (productParams.searchTerm) {
      params = params.append('searchTerm', productParams.searchTerm);
    }

    if (productParams.orderBy) {
      params = params.append('orderBy', productParams.orderBy);
    }

    return this.http
      .get<
        ApiResponse<PaginatedResponse<GetStockTransactionDto>>
      >(`${this.baseUrl}/GetAlStockTransactions`, { params })
      .pipe(map((res) => res.data));
  }
  AddStockTransaction(StockTransaction: AddStockTransaction): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/AddStockTransaction`, {
      StockTransaction,
    });
  }

  DeleteStockTransaction(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/DeleteStockTransaction/${id}`);
  }
  getProductsLookup(): Observable<ApiResponse<GetProductsLookUpDTO[]>> {
    return this.http.get<ApiResponse<GetProductsLookUpDTO[]>>(`${this.baseUrl}/GetProductsLookUp`);
  }
}
