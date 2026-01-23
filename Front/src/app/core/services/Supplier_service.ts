import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import { PaginatedResponse } from '../../shared/shared_models/PaginatedResponse';
import { ProductParams } from '../models/ProductParams-model';

import { AddSupplierDTO, SupplierDTO, UpdateSupplierDTO } from '../models/Supplier.rmodel';
@Injectable({ providedIn: 'root' })
export class SupplierService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Suppliers`;

  GetAlSuppliers(productParams: ProductParams): Observable<PaginatedResponse<SupplierDTO>> {
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
        ApiResponse<PaginatedResponse<SupplierDTO>>
      >(`${this.baseUrl}/GetAlSuppliers`, { params })
      .pipe(map((res) => res.data));
  }
  AddSupplier(
    SupplierDTO: AddSupplierDTO,
    CountryCode: String,
    VatNumber: string,
  ): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/AddSupplier`, {
      SupplierDTO,
      CountryCode,
      VatNumber,
    });
  }

  UpdateSupplier(UpdateSupplierDTO: UpdateSupplierDTO): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.baseUrl}/UpdateSupplier`, {
      UpdateSupplierDTO,
    });
  }

  DeleteSupplier(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/DeleteSupplier/${id}`);
  }
}
