import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  showError(errorMessage: string) {
    Swal.fire({
      icon: 'error',
      title: errorMessage,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: '#FF0B0B', 
      iconColor: '#22c55e'
    });  }

  // Success Notification (Toast style)
  showSuccess(message: string) {
    Swal.fire({
      icon: 'success',
      title: message,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: '#f0fdf4', // Light green
      iconColor: '#22c55e'
    });
  }

  // Confirmation Dialog (For Delete)
  confirmDelete(title: string, callback: () => void) {
    Swal.fire({
      title: title,
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444', // Red
      cancelButtonColor: '#64748b', // Slate
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        callback();
      }
    });
  }
}