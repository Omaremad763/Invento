import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { ConfirmEmailDTO } from '../../../shared/shared_models/Auth-models';
import { AuthService } from '../../../shared/shared_services/auth.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule, AuthPhotoComponent, RouterLink],
  templateUrl: './ConfirmEmail.html',
})
export class ConfirmEmailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private router = inject(Router);

  status: 'loading' | 'success' | 'error' = 'loading';
  message: string = '';

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const Token = this.route.snapshot.queryParamMap.get('token');

    if (!userId || !Token) {
      this.status = 'error';
      this.message = 'Invalid verification link.';
      return;
    }
    const ConfirmData: ConfirmEmailDTO = {
      userID: userId,
      token: Token,
    };
    this.authService.confirmEmail(ConfirmData).subscribe({
      next: (res) => {
        this.status = 'success';
        this.message = 'Your email has been verified successfully!';
      },
      error: (err) => {
        this.status = 'error';
        this.message = err.error?.message || 'Verification failed or link expired.';
      },
    });
  }
}
