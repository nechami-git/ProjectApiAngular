import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RegisterModel } from '../../Models/register.model'
import { Observable, tap } from 'rxjs';
import { LoginModel,UserToken } from '../../Models/login.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  http:HttpClient = inject(HttpClient)
  BASE_URL:string= 'http://localhost:8080/api/User'

  currentUser = signal<UserToken | null>(this.getUserFromStorage());

  isLoggedIn = computed(() => !!this.currentUser());

  isAdmin = computed(() => this.currentUser()?.role === 'Manager');

  register(user:RegisterModel): Observable<void>{
    return this.http.post<void>(`${this.BASE_URL}/register`, user);
  }

  login(user:LoginModel):Observable<any>{
    return this.http.post<any>(`${this.BASE_URL}/login`,user).pipe(
      tap((res:any)=>{
        if(res && res.token){
          localStorage.setItem('token', res.token); // שמירה בזיכרון
          const decodedUser = this.decodeToken(res.token);
          this.currentUser.set(decodedUser);
        }
      })
    )
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUser.set(null); // איפוס הסיגנל
  }
  //האם רשום
  isAuthenticated(): boolean{
    return !!localStorage.getItem('token');
}

  getUserFromStorage(): UserToken | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    return this.decodeToken(token);
  }
  
  decodeToken(token: string): UserToken {
    const decoded: any = jwtDecode(token);
    console.log('Decoded Token Data:', decoded);
    return {
      id:decoded.id,
      email: decoded.email,
      role: decoded.role,
      name: decoded.unique_name || decoded.name || 'אורח'
    };
  }
}
