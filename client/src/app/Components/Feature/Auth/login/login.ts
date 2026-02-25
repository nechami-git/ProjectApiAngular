import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute ,RouterLink} from '@angular/router';
import { AuthService } from '../../../../Services/auth/authService';
import { LoginModel } from '../../../../Models/login.model';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';


@Component({
  selector: 'app-login',
  imports: [CommonModule,ReactiveFormsModule,CardModule, InputTextModule, PasswordModule, ButtonModule, ToastModule,RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class Login {
  //inject
  router: Router = inject(Router)
  activatedRoute: ActivatedRoute = inject(ActivatedRoute)
  authSrv: AuthService = inject(AuthService)
  messageService: MessageService = inject(MessageService)

  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');


  frmLogin: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  })

  enter() {
    if (this.frmLogin.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'שגיאה', detail: 'נא למלא את כל השדות' });
      return
    }

    let user: LoginModel = {
      email: this.frmLogin.controls['email'].value,
      password: this.frmLogin.controls['password'].value
    }
    this.authSrv.login(user).subscribe({
      next: (res:any) => {
 

        setTimeout(() => {
          this.router.navigate(['/gifts']); 
        }, 2000);
      },
      error: (err) => {
        //this.messageService.add({ severity: 'error', summary: 'תקלה', detail: err.error });
        this.isLoading.set(false)
        const msg = err.error?err.error: 'שגיאה בהתחברות, נא לנסות שוב';
        this.errorMessage.set(msg)
        console.error(err);
      }

    })
  }
}
