
export interface PaginatedResponse<T> {
  data: T[];              
  pageNumber: number;
  pageSize: number;
  totalCount: number;     
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
  errors: string[];
}