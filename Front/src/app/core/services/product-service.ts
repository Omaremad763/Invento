import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AddProductDto, Product, UpdateProductDto } from '../../core/models/Product.model';
import { ProductParams } from '../../core/models/ProductParams-model';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import { PaginatedResponse } from './../../shared/shared_models/PaginatedResponse';
@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Products`;

  getAllProducts(productParams: ProductParams): Observable<PaginatedResponse<Product>> {
    let params = new HttpParams();

    // 1. Pagination
    if (productParams.pageNumber) {
      params = params.append('pageNumber', productParams.pageNumber.toString());
    }

    if (productParams.pageSize) {
      params = params.append('pageSize', productParams.pageSize.toString());
    }

    // 2. Search & Filtering
    if (productParams.searchTerm) {
      params = params.append('searchTerm', productParams.searchTerm);
    }

    if (productParams.categoryId) {
      params = params.append('categoryId', productParams.categoryId.toString());
    }

    // 3. Sorting
    if (productParams.orderBy) {
      params = params.append('orderBy', productParams.orderBy);
    }

    return this.http
      .get<ApiResponse<PaginatedResponse<Product>>>(`${this.baseUrl}/GetAlProducts`, { params })
      .pipe(map((res) => res.data));
  }

  getProductById(id: string): Observable<ApiResponse<Product>> {
    return this.http.get<ApiResponse<Product>>(`${this.baseUrl}/GetProductByID/${id}`);
  }

  addProduct(product: AddProductDto): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/AddProduct`, { product });
  }

  updateProduct(product: UpdateProductDto): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.baseUrl}/UpdateProduct`, { product });
  }

  deleteProduct(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/DeleteProduct/${id}`);
  }
}
