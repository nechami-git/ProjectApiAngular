import { Component, inject } from '@angular/core';
import { GiftService } from '../../../../Services/gift/gift-service';
import { MessageService } from 'primeng/api';
import { GiftFilterAdmin, GiftModel } from '../../../../Models/gift.model';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { DataViewModule } from 'primeng/dataview';

@Component({
  selector: 'app-raffele',
  imports: [CommonModule, FormsModule, ToastModule, ButtonModule, InputTextModule, TagModule, TooltipModule],
  templateUrl: './raffele.html',
  styleUrls: ['./raffele.scss'],
})
export class Raffele {
  giftSrv: GiftService = inject(GiftService);
  messageService: MessageService = inject(MessageService);
  gifts: GiftModel[] = [];
  rafflingGiftId: number | null = null;
  
  myFilter: GiftFilterAdmin = {
    name: '',
    donorName: ''
  };

  ngOnInit() {
    this.getAll();
  }

  getAll() {
    this.giftSrv.getAdminGifts(this.myFilter).subscribe({
      next: (data) => {
        this.gifts = data;
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת המתנות'
        });
      }
    });
  }

  performRaffle(giftId: number) {
    if (this.rafflingGiftId) return; // Prevent multiple clicks
    
    this.rafflingGiftId = giftId; // Start animation
    
    // Artificial delay for the "slot machine" effect
    setTimeout(() => {
      this.giftSrv.performRaffle(giftId).subscribe({
        next: (updatedGift) => {
          const index = this.gifts.findIndex(g => g.id === giftId);
          if (index !== -1) {
            this.gifts[index] = updatedGift;
          }
          
          this.messageService.add({
            severity: 'success',
            summary: '🎉 יש לנו זוכה!',
            detail: `הזוכה במתנה "${updatedGift.name}" הוא: ${updatedGift.winnerName}`,
            life: 5000
          });
          
          setTimeout(() => {
             this.rafflingGiftId = null; // Stop animation
          }, 1500); // Show result before stopping animation completely
        },
        error: (err) => {
          this.rafflingGiftId = null;
          this.messageService.add({
            severity: 'error',
            summary: 'שגיאה',
            detail: err.error?.message || 'הגרלה נכשלה'
          });
        }
      });
    }, 2000); // Run animation for at least 2 seconds
  }

  generateReport() {
    this.giftSrv.generateReport().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Winners_Report_${new Date().toISOString().slice(0, 10)}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
         this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'נכשל בהורדת הדוח' });
      }
    });
  }
}
        
 

