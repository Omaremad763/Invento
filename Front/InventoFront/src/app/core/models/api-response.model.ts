// الواجهة الأساسية لأي Response يحتوي على Pagination
export interface PaginatedResponse<T> {
  data: T[];              // مصفوفة البيانات (منتجات، فئات، إلخ)
  pageNumber: number;
  pageSize: number;
  totalCount: number;     // إجمالي العناصر (مهم جداً للـ Table في الفرونت)
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// واجهة لأي Response عادي (بدون Pagination)
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}