import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';
import Swal from 'sweetalert2';
import { environment } from '../../../environment';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { ExternalAuthDTO, ExternalAuthResponse } from '../../../shared/shared_models/Auth-models';
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
  state: string | null;
  ActivatedRoute = inject(ActivatedRoute);
  form = this.fb.group({
    Username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });
  ngOnInit() {
    this.ActivatedRoute.queryParamMap.pipe(take(1)).subscribe((params) => {
      this.state = params.get('state');
      const _code = params.get('code');
      if (!_code) {
        return;
      }
      const githubCode: ExternalAuthDTO = {
        code: _code,
      };

      this.HandleExternalAuth(githubCode);
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.auth.register(this.form.value as any).subscribe({
      next: (res) => {
        if (res.isAuthenticated) {
          Swal.fire({
            title: 'Registraion Successful!',
            text: 'Please check your email to confirm your account before logging in',
            icon: 'info',
            confirmButtonColor: '#4F46E5',
            confirmButtonText: 'Go to Messages Inbox',
          }).then(() => {
            window.open('http://localhost:1080', '_blank');
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
    });
  }

  ExternalAuth() {
    const clientId = `${environment.githubID}`;
    const currentOrigin = window.location.origin;
    const redirectUri = encodeURIComponent(`${currentOrigin}/register`);
    const scope = 'user:email';
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=${scope}`;
  }
  private HandleExternalAuth(code: ExternalAuthDTO) {
    this.router.navigate([], {
      queryParams: { code: null },
      queryParamsHandling: 'merge',
      replaceUrl: true,
    });

    this.auth.AuthWithGithub(code).subscribe({
      next: (res: ExternalAuthResponse) => {
        if (res.isAuthenticated == true) {
          localStorage.setItem('token', res.token);
          Swal.fire({
            icon: 'success',
            title: this.state === 'login_request' ? 'Login Successful' : 'Registration Successful',
            text:
              this.state === 'login_request'
                ? 'Redirecting you to the Dashboard...'
                : 'Redirecting you to the Login...',
            timer: 2000,
            timerProgressBar: true,
            showConfirmButton: false,
          }).then(() => {
            if (this.state === 'login_request') {
              this.router.navigate(['/dashboard'], { replaceUrl: true });
            } else {
              this.router.navigate(['/login'], { replaceUrl: true });
            }
          });
        } else {
          Swal.fire({
            icon: 'warning',
            title: 'Authentication Failed',
            text: 'Your account could not be verified. Please try again.',
            confirmButtonColor: '#f8bb86',
          });
        }
      },
    });
  }
}
