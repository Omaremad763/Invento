import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { environment } from '../../../environment';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { LoginDto } from '../../../shared/shared_models/Auth-models';
import { AuthService } from '../../../shared/shared_services/auth.service';
import { LoadingService } from '../../../shared/shared_services/loading.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AuthPhotoComponent],
  templateUrl: './login.html',
})
export class LoginComponent {
  fb = inject(FormBuilder);
  auth = inject(AuthService);
  router = inject(Router);
  LoadingService = inject(LoadingService);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  onSubmit() {
    if (this.form.invalid) return;

    const loginData: LoginDto = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
    };

    this.auth.login(loginData).subscribe({
      next: (res: any) => {
        if (res.isAuthenticated === true) {
          Swal.fire({
            title: 'Login Successful!',
            text: 'You are now logged in.',
            icon: 'success',
            timer: 2000,
            timerProgressBar: true,
            showConfirmButton: false,
          }).then(() => {
            this.router.navigate(['/dashboard'], { replaceUrl: true });
          });
        } else {
          Swal.fire({
            title: 'Login Failed',
            text: 'Invalid email or password.',
            icon: 'error',
            confirmButtonColor: '#EF4444',
          });
        }
      },
      error: (err) => {
        Swal.fire({
          title: 'Server Error',
          text: 'Could not connect to the server. Please try again later.',
          icon: 'warning',
          confirmButtonColor: '#F59E0B',
        });
      },
    });
  }

  // LoginWithGoogle() {
  //   // @ts-ignore
  //   google.accounts.id.initialize({
  //     client_id: 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com',
  //     callback: (response: any) => this.handleGoogleResponse(response),
  //   });
  //   // @ts-ignore
  //   google.accounts.id.prompt();
  // }
  ExternalAuth() {
    const clientId = `${environment.githubID}`;
    const redirectUri = encodeURIComponent('http://localhost:4200/login');
    const scope = 'user:email';

    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=${scope}`;
  }
  private handleGoogleResponse(response: any) {
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
