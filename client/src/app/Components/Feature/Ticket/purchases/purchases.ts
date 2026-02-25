import { Component, inject } from '@angular/core';
import { PurchasesService } from '../../../../Services/purchases/purchases-service';
import { PurchaseModel, GiftPurchasersDTO, PurchaseFilter, PurchaseSortOption } from '../../../../Models/parchaces.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TooltipModule } from 'primeng/tooltip';
import { GiftService } from '../../../../Services/gift/gift-service';

@Component({
  selector: 'app-purchases',
  imports: [ CommonModule,FormsModule,ToastModule,TableModule,ToolbarModule,ButtonModule,InputTextModule, DropdownModule, TooltipModule],
  providers: [MessageService],
  templateUrl: './purchases.html',
  styleUrls: ['./purchases.scss'],
})
export class Purchases {

  purchacesSrv: PurchasesService = inject(PurchasesService);
  giftSrv: GiftService = inject(GiftService);
  messageService: MessageService = inject(MessageService);

  purchases: PurchaseModel[] = [];
  filteredPurchases: PurchaseModel[] = [];
  groupedPurchases: { id: string; date: Date; purchases: PurchaseModel[]; totalAmount: number; totalQuantity: number }[] = [];
  purchasesByGift: GiftPurchasersDTO[] = [];
  filter: PurchaseFilter = {sortBy: undefined};
  searchText: string = '';

  sortOptions = [
    { label: 'ללא מיון', value: undefined },
    { label: 'הכי יקר', value: PurchaseSortOption.Expensive },
    { label: 'הכי נרכש', value: PurchaseSortOption.MostPurchased }
  ];

  ngOnInit() {
    this.getAllPurchases();
  }

  getAllPurchases() {
    this.purchacesSrv.getAllparchaces(this.filter).subscribe({
      next: (data) => {
        this.purchases = data;
        this.filteredPurchases = data;
        this.groupPurchasesByDate();
        console.log('Purchases loaded:', this.purchases);
      },
      error: (err) => {
        console.error('Error fetching purchases:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת הרכישות: ' + (err.message || err.status)  
      });
    }
    });
  }

  filterPurchases() {
    if (!this.searchText || this.searchText.trim() === '') {
      this.filteredPurchases = this.purchases;
    } else {
      const searchLower = this.searchText.toLowerCase();
      this.filteredPurchases = this.purchases.filter(p => 
        (p.buyerName?.toLowerCase().includes(searchLower)) ||
        (p.giftName?.toLowerCase().includes(searchLower)) ||
        (p.buyerEmail?.toLowerCase().includes(searchLower)) ||
        (p.buyerPhone?.includes(searchLower))
      );
    }
    this.groupPurchasesByDate();
  }

  groupPurchasesByDate() {
    const groups = new Map<string, { id: string; date: Date; purchases: PurchaseModel[]; totalAmount: number; totalQuantity: number }>();

    this.filteredPurchases.forEach(purchase => {
      if (!purchase) return;
      
      let dateKey = 'unknown-date';
      let fullDate = new Date();

      if (purchase.purchaseDate) {
        const d = new Date(purchase.purchaseDate);
        if (!isNaN(d.getTime())) {
          dateKey = d.toISOString();
          fullDate = d;
        }
      }

      if (!groups.has(dateKey)) {
        groups.set(dateKey, {
          id: dateKey,
          date: fullDate,
          purchases: [],
          totalAmount: 0,
          totalQuantity: 0
        });
      }

      const currentGroup = groups.get(dateKey)!;
      currentGroup.purchases.push(purchase);
      currentGroup.totalAmount += (purchase.totalPrice || 0);
      currentGroup.totalQuantity += (Number(purchase.quantity) || 0);
    });

    this.groupedPurchases = Array.from(groups.values()).sort((a, b) => {
      const timeA = a.date ? a.date.getTime() : 0;
      const timeB = b.date ? b.date.getTime() : 0;
      return timeB - timeA;
    });
  }

  calculateTotalRevenue(): number {
    return this.filteredPurchases.reduce((sum, purchase) => sum + purchase.totalPrice, 0);
  }

  generateSalesReport() {
    this.giftSrv.generateReport().subscribe({
      next: (blob: Blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Sales_Report_${new Date().toISOString().slice(0, 10)}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        
        this.messageService.add({
          severity: 'success',
          summary: 'דוח הורד בהצלחה',
          detail: 'קובץ דוח המכירות הורד למחשב שלך',
          life: 5000
        });
      },
      error: (err: any) => {
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: err.error?.message || 'הורדת דוח המכירות נכשלה'
        });
      }
    });
  }
}
