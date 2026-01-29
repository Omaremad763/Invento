
export interface Product {
  id: string;
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
  categoryName: string;
}

export interface AddProductDto {
  name: string;
  sku: string;
  price: number;
  stockQuantity: number;
  categoryId: string;
}

export interface UpdateProductDto {
  id: string;
  name?: string | null;
  sku?: string | null;
  price?: number | null;
  categoryId?: string | null;
}