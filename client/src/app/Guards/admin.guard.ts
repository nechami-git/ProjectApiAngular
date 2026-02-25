import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth/authService'; // וודא שהנתיב נכון
import { MessageService } from 'primeng/api'; // אופציונלי: להודעה יפה

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isAdmin()) {
    return true; // יש אישור להיכנס
  } 

  alert('אין לך הרשאת מנהל לצפייה בדף זה!'); 

  router.navigate(['/']); 
  
  return false; 
};