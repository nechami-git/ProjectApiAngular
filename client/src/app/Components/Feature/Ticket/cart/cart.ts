import { Component, inject } from '@angular/core';
import { CartItem, UpdateCartItem } from '../../../../Models/ticket.model';
import { CartService } from '../../../../Services/cart/cart-service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [ToolbarModule, RouterModule, CommonModule, ButtonModule, TableModule, ToastModule, ConfirmDialogModule, InputNumberModule, FormsModule],
  templateUrl: './cart.html',
  styleUrls: ['./cart.scss'],
})
export class Cart {
  cartService: CartService = inject(CartService);
  cartItems: CartItem[] = [];
  messageService: MessageService = inject(MessageService);
  isLoading: boolean = false;

  get totalAmount(): number {
    return this.cartItems.reduce((sum, item) => sum + (item.ticketPrice * item.quantity), 0);
  }
  ngOnInit() {
    this.getCart()
  }

  updateQuantity(item: CartItem, val: string | number | null) {
    const quantity = Number(val);
    if (!isNaN(quantity) && quantity > 0) {
      item.quantity = quantity;
      item.totalPrice = item.quantity * item.ticketPrice;
      this.updateCartItem({ giftId: item.giftId, quantity });
    }
  }

  deleteItem(item: CartItem) {
    const previousItems = [...this.cartItems];
    this.cartItems = this.cartItems.filter(i => i.id !== item.id);
    this.cartService.removeFromCart(item.id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה',
          detail: 'הפריט הוסר מהעגלה'
        });
      },
      error: (err) => {
        this.cartItems = previousItems;
        console.error('Error removing cart item:', err);
        const serverMsg = err.error?.message || 'הפריט לא נמצא בשרת, מסנכרן נתונים...';
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: serverMsg });

        this.getCart();
      }
    });
  }

  getCart() {
    this.cartService.getCart().subscribe({
      next: (data) => {
        this.cartItems = data;
        console.log(data);
      },
      error: (err) => {
        console.error('Error fetching cart items:', err);
      }
    })
  }

  updateCartItem(item: UpdateCartItem) {
    this.cartService.updateCartItem(item).subscribe({
      next: () => {
        console.log(`Updated itemId ${item.giftId} to quantity ${item.quantity}.`);

        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה',
          detail: 'פריט העדכון בעגלה בוצע בהצלחה'
        });
      },
      error: (err) => {

        console.error('Error updating cart item:', err);

        const serverMsg = err.error?.message || err.message || 'אירעה שגיאה בעדכון';

        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: serverMsg
        });
      }
    });
  }
  checkout() {
    this.cartService.checkout().subscribe({
      next: () => {
        this.cartItems = [];
        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה',
          detail: 'הרכישה בוצעה בהצלחה! 🎁'
        });

        console.log('Checkout successful and cart cleared locally.');
      },
      error: (err) => {
        console.error('Error during checkout:', err);
        const serverMsg = err.error?.message || 'אירעה שגיאה בתהליך הרכישה';
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: serverMsg
        });
      }
    });
  }
}