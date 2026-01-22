export interface Category {
  id: string;
  categoryName: string;
}

export interface UpdateCategoryDto {
  id: string;
  name: string | null;
}
export interface AddCategoryDto {
  name: string | null;
}
