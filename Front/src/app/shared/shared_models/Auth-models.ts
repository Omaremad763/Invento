export interface ExternalAuthDTO {
  code: string;
}

export interface ExternalAuthResponse {
  isAuthenticated: boolean;
  token: string;
}

export interface SendEmailDto {
  userName: string;
  link: string;
}

export interface ConfirmEmailDTO {
  userID: string;
  token: string;
}

export interface ConfirmResponse {
  isAuthenticated: boolean;
  confirmMessageResult: string;
}

export interface LoginDto {
  email: string;
  password: string;
  CaptachaToken: string;
}

export interface LoginResponse {
  isAuthenticated: boolean;
  token: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  userName: string;
}

export interface RegisterResponse {
  isAuthenticated: boolean;
  confirmMessageRequest: string;
}
