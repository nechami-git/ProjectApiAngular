import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../../Services/auth/authService';
import { RegisterModel } from '../../../../Models/register.model';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, CardModule, InputTextModule, PasswordModule, ButtonModule, ToastModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
})
export class Register {
  //inject
  authSrv: AuthService = inject(AuthService)
  messageService: MessageService = inject(MessageService);//בשביל ההודעות
  router: Router = inject(Router);//סרביס שאחראי על הפעולות של ניווטים
  activatedRoute: ActivatedRoute = inject(ActivatedRoute);//סרביס שמכיל מידע על הניווט הנוכחי שעומדים עליו

  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');


  //form
  frmRegister: FormGroup = new FormGroup({
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    phone: new FormControl('', [Validators.required]),
  });

  saveUser() {
    if (this.frmRegister.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'נא למלא את כל השדות' });
      return
    }
    let user: RegisterModel = {
      firstName: this.frmRegister.controls['firstName'].value,
      lastName: this.frmRegister.controls['lastName'].value,
      email: this.frmRegister.controls['email'].value,
      password: this.frmRegister.controls['password'].value,
      phone: this.frmRegister.controls['phone'].value,
    }

    this.isLoading.set(true)
    this.errorMessage.set('')

    this.authSrv.register(user).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.successMessage.set('ההרשמה בוצעה בהצלחה! מעביר אותך להתחברות...');

        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה',
          detail: 'נרשמת בהצלחה!'
        });

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = err.error?.message || 'שגיאה בהרשמה, נא לנסות שוב';
        this.errorMessage.set(msg); 
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה בתהליך ההרשמה',
          detail: msg
        });

        console.error('Registration error:', err);
      }
    });
  }


}
