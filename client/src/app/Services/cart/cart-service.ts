import { HttpClient } from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { AddToCart, CartItem, UpdateCartItem } from '../../Models/ticket.model';


@Injectable({
  providedIn: 'root',
})
export class CartService {
  private BASE_URL = 'http://localhost:8080/api/Cart';

  constructor(private http: HttpClient) { }
  cartItems = signal<CartItem[]>([]);

  cartCount = computed(() =>
    this.cartItems().reduce((count, item) => count + item.quantity, 0)
  );

  totalCartPrice = computed(() =>
    this.cartItems().reduce((total, item) => total + item.totalPrice, 0)
  );

  getCart(): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(this.BASE_URL).pipe(
      tap({
        next: (items) => this.cartItems.set(items),
        error: () => this.cartItems.set([])

      })
    );
  }

  addToCart(item: AddToCart): Observable<void> {
    return this.http.post<void>(`${this.BASE_URL}/add`, item).pipe(
       tap({
        next: () => this.getCart().subscribe(),
        error: () => {}
      }) 
    );
  }
  updateCartItem(item:UpdateCartItem): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/update`, { itemId: item.giftId, quantity: item.quantity }).pipe(
      tap({
        next: () => this.getCart().subscribe(),
        error: () => {}
      })
    );
  }

  removeFromCart(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/${itemId}`).pipe(
      tap({
        next: () => this.getCart().subscribe(),
        error: () => {}
      })
    );
  }
  checkout(): Observable<void> {
   return this.http.post<void>(`${this.BASE_URL}/checkout`, {}).pipe(
      tap({
        next: () => this.cartItems.set([]),
        error: () => {}
      })
    );
  }
}