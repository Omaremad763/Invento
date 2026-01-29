import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environment';
import { ApiResponse } from '../../shared/shared_models/api-response.model';
import { PaginatedResponse } from '../../shared/shared_models/PaginatedResponse';
import { Category, UpdateCategoryDto } from '../models/Category.model';
import { ProductParams } from '../models/ProductParams-model';
@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Categories`;

  getAllCategories(productParams: ProductParams): Observable<PaginatedResponse<Category>> {
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

    if (productParams.categoryId) {
      params = params.append('categoryId', productParams.categoryId.toString());
    }

    if (productParams.orderBy) {
      params = params.append('orderBy', productParams.orderBy);
    }

    return this.http
      .get<ApiResponse<PaginatedResponse<Category>>>(`${this.baseUrl}/GetAlCategories`, { params })
      .pipe(map((res) => res.data));
  }
  addCategory(CategoryName: String): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/AddCategory`, { CategoryName });
  }

  updateCategory(Category: UpdateCategoryDto): Observable<ApiResponse<void>> {
    return this.http.put<ApiResponse<void>>(`${this.baseUrl}/UpdateCategory`, { Category });
  }

  deleteCategory(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/DeleteCateogry/${id}`);
  }
}
