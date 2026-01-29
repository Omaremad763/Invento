export interface SupplierDTO {
  id: string;
  name: string;
  contactEmail: string;
  phoneNumber: string;
  vatstatus: string;
}

export interface AddSupplierDTO {
  Name: string;
  ContactEmail: string;
  PhoneNumber: string;
}

export interface UpdateSupplierDTO {
  id: string;
  name: string;
  contactEmail: string;
  phoneNumber: string;
}
