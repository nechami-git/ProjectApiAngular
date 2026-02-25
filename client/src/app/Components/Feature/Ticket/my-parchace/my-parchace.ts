import { Component, inject } from '@angular/core';
import { PurchasesService } from '../../../../Services/purchases/purchases-service';
import { PurchaseModel } from '../../../../Models/parchaces.model';
import { CommonModule } from '@angular/common'; 

interface GroupedPurchase {
  id: string;
  date: Date;
  items: PurchaseModel[];
  totalAmount: number;
  totalQuantity: number; 
}

@Component({
  selector: 'app-my-parchace',
  imports: [CommonModule],
  templateUrl: './my-parchace.html',
  styleUrls: ['./my-parchace.scss'],
})
export class MyParchace {
  purchaceSrv:PurchasesService = inject(PurchasesService);
  parchaces: PurchaseModel[] = [];

  groupedPurchases: GroupedPurchase[] = [];

  ngOnInit() {
    this.getMyPurchases();
  }

  getMyPurchases() {
    this.purchaceSrv.getMyPurchases().subscribe({
      next: (data) => {
        //this.parchaces = data;
        if (data && data.length > 0) {
          this.groupPurchases(data);
        }
        console.log( data);
      },
      error: (err) => {
        console.error('Error fetching purchases:', err.message?err.message:'אירעה שגיאה בטעינת ההזמנות');
      }
    })
  };


private groupPurchases(data: PurchaseModel[]) {
    const groups = new Map<string, GroupedPurchase>();

    data.forEach(item => {
      if (!item) return;
      // זיהוי תאריך ושעה (כולל דקות/שניות להפרדת הזמנות)
      let dateKey = 'unknown-date';
      let fullDate = new Date();

      if (item.purchaseDate) {
        const d = new Date(item.purchaseDate);
        if (!isNaN(d.getTime())) {
          dateKey = d.toISOString();
          fullDate = d;
        }
      }

      // יצירת קבוצה חדשה אם צריך
      if (!groups.has(dateKey)) {
        groups.set(dateKey, {
          id: dateKey,
          date: fullDate,
          items: [],
          totalAmount: 0,
          totalQuantity: 0 // <--- אתחול מונה הכמות
        });
      }

      const currentGroup = groups.get(dateKey)!;
      currentGroup.items.push(item);
      
      // חישובים:
      // 1. הוספת המחיר לסכום הכולל
      currentGroup.totalAmount += (item.totalPrice || 0);
      
      // 2. הוספת הכמות לסך הפריטים (ממיר למספר ליתר ביטחון)
      const qty = Number(item.quantity) || 0;
      currentGroup.totalQuantity += qty;
    });

    // מיון לפי תאריך (מהחדש לישן)
    this.groupedPurchases = Array.from(groups.values()).sort((a, b) => {
      const timeA = a.date ? a.date.getTime() : 0;
      const timeB = b.date ? b.date.getTime() : 0;
      return timeB - timeA;
    });
  }

}
