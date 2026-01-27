import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { environment } from '../../../environment';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { AuthService } from '../../../shared/shared_services/auth.service';
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AuthPhotoComponent],
  templateUrl: './register.html',
})
export class RegisterComponent {
  fb = inject(FormBuilder);
  auth = inject(AuthService);
  router = inject(Router);
  form = this.fb.group({
    Username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });
  onSubmit() {
    if (this.form.invalid) return;

    this.auth.register(this.form.value as any).subscribe({
      next: (res) => {
        if (res.isAuthenticated) {
          Swal.fire({
            title: 'Login Successful!',
            text: 'Please check your email to confirm your account before logging in',
            icon: 'info',
            confirmButtonColor: '#4F46E5',
            confirmButtonText: 'Go to Login',
          }).then(() => {
            this.router.navigate(['/login']);
          });
        } else {
          Swal.fire({
            title: 'Authentication Failed',
            text: res.confirmMessageRequest,
            icon: 'error',
            confirmButtonColor: '#EF4444',
            confirmButtonText: 'Retry',
          });
        }
      },
      error: (err) => {
        Swal.fire({
          title: 'Server Error',
          text: 'Something went wrong. Please try again later.',
          icon: 'error',
          confirmButtonColor: '#EF4444',
          confirmButtonText: 'OK',
        });
      },
    });
  }

  // registerWithGoogle() {
  //   // @ts-ignore
  //   google.accounts.id.initialize({
  //     client_id: 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com',
  //     callback: (response: any) => this.HandleExternalAuth(response),
  //   });
  //   // @ts-ignore
  //   google.accounts.id.prompt();
  // }
  ExternalAuth() {
    const clientId = `${environment.githubID}`;
    // يفضل وضعه في environment.ts
    const redirectUri = encodeURIComponent('http://localhost:4200/login');
    const scope = 'user:email';

    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=${scope}`;
  }
  private HandleExternalAuth(response: any) {
    const token = response.credential;
    this.auth.AuthWithGithub(token).subscribe({
      next: (res: { token: string }) => {
        localStorage.setItem('token', res.token);
        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        console.error('Google Auth Error:', err);
      },
    });
  }
}
