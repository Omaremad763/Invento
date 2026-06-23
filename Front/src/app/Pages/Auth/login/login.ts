import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, inject } from '@angular/core'; // أضفنا AfterViewInit
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { environment } from '../../../environment';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { LoginDto, LoginResponse } from '../../../shared/shared_models/Auth-models';
import { AuthService } from '../../../shared/shared_services/auth.service';
import { AuthStateService } from '../../../shared/shared_services/AuthStateService';
import { LoadingService } from '../../../shared/shared_services/loading.service';

// تعريف الكائن العالمي لجوجل
declare var grecaptcha: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AuthPhotoComponent],
  templateUrl: './login.html',
})
export class LoginComponent implements AfterViewInit {
  // تنفيذ الواجهة
  fb = inject(FormBuilder);
  auth = inject(AuthService);
  router = inject(Router);
  LoadingService = inject(LoadingService);
  ActivatedRoute = inject(ActivatedRoute);
  authState = inject(AuthStateService);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  siteKey = environment.recaptchaSiteKey;
  token: string = '';
  widgetId: any = null;

  ngAfterViewInit() {
    this.renderCaptcha();
  }

  renderCaptcha() {
    setTimeout(() => {
      if (typeof grecaptcha !== 'undefined' && document.getElementById('InventoCaptcha')) {
        try {
          const container = document.getElementById('InventoCaptcha');
          if (container) container.innerHTML = '';

          this.widgetId = grecaptcha.render('InventoCaptcha', {
            sitekey: this.siteKey,
            theme: 'light',
          });
        } catch (error) {
          console.warn('ReCaptcha render error:', error);
        }
      }
    }, 300);
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.token = grecaptcha.getResponse(this.widgetId);

    if (!this.token) {
      Swal.fire({
        title: 'Security Check',
        text: 'Please complete the ReCaptcha to proceed.',
        icon: 'warning',
        confirmButtonColor: '#3B82F6',
      });
      return;
    }

    const loginData: LoginDto = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
      CaptachaToken: this.token,
    };

    this.auth.login(loginData).subscribe({
      next: (res: LoginResponse) => {
        if (res.isAuthenticated === true) {
          Swal.fire({
            title: 'Login Successful!',
            text: 'You are now logged in.',
            icon: 'success',
            timer: 2000,
            timerProgressBar: true,
            showConfirmButton: false,
          }).then(() => {
            this.authState.setAuth(true);
            this.router.navigate(['/dashboard'], { replaceUrl: true });
          });
        } else {
          grecaptcha.reset(this.widgetId);
          Swal.fire({
            title: 'Login Failed',
            text: 'Invalid email or password.',
            icon: 'error',
            confirmButtonColor: '#EF4444',
          });
        }
      },
      error: () => {
        grecaptcha.reset(this.widgetId);
      },
    });
  }

  ExternalAuth() {
    const clientId = `${environment.githubID}`;
    const redirectUri = encodeURIComponent('http://localhost:4200/register');
    const scope = 'user:email';
    const state = 'login_request';
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=${scope}&state=${state}`;
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
}
