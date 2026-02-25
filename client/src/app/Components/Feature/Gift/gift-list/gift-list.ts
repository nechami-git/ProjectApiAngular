import { Component, inject } from '@angular/core';
import { GiftFilter, GiftModel } from '../../../../Models/gift.model';
import { GiftService } from '../../../../Services/gift/gift-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { CartService } from '../../../../Services/cart/cart-service';
import { AuthService } from '../../../../Services/auth/authService';
import { MessageService } from 'primeng/api';
import { AddToCart } from '../../../../Models/ticket.model';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-gift-list',
  imports: [CommonModule, FormsModule, DataViewModule, TagModule, ButtonModule, InputTextModule, TooltipModule,ToastModule],
  templateUrl: './gift-list.html',
  styleUrls: ['./gift-list.scss'],
})
export class GiftList {
  giftService: GiftService = inject(GiftService);
  cartService: CartService = inject(CartService);
  authService: AuthService = inject(AuthService);

  messageService: MessageService = inject(MessageService);
  raffleed: boolean = false;

  gifts: GiftModel[] = [];

  filter: GiftFilter = {
    name: '',
    CategoryName: '',
    TicketPrice: 0
  };

  ngOnInit() {
    this.getAll();
  }
  isRaffled(gift: GiftModel): boolean {
    return gift.winnerName != null;
  }

  getAll() {
    this.giftService.getAll(this.filter).subscribe({
      next: (data) => {
        this.gifts = data;
        console.log('המתנות נטענו בהצלחה:', data);
      },
      error: (err) => {
        console.error('שגיאה בטעינת המתנות:', err);
      }
    })
  }

  clearFilter(): void {
    this.filter = { name: '', CategoryName: '', TicketPrice: 0 };
    this.getAll();
  }

  addToCart(gift: AddToCart) {
    this.cartService.addToCart(gift).subscribe({
      next: () => {
        this.messageService.add({ 
          severity: 'success',
          summary: 'הצלחה',
          detail: 'המתנה נוספה לסל בהצלחה' 
        });
      },
      error: (err) => {
       this.messageService.add({ 
        severity: 'error', 
        summary: 'שגיאה', 
        detail: err.error?.message })
      }
    })
  }
}
