export interface AddStockTransaction {
  productId: string;
  quantity: number;
  transactionType: StockTransactionTypeEnum;
}
export interface GetStockTransactionDto {
  id: string; // Guid
  productName: string;
  appliedQuantity: number;
  stockTransactionType: string;
  createdAt: string;
}
export enum StockTransactionTypeEnum {
  Purchase = 1,
  Sale = 2,
}
export interface GetProductsLookUpDTO {
  id: string;
  Name: string;
}
