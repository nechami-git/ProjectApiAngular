import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { GiftPurchasersDTO, PurchaseFilter, PurchaseModel } from '../../Models/parchaces.model';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PurchasesService {
  BASE_URL = 'http://localhost:8080/api/purchase';
  http: HttpClient = inject(HttpClient);

  parchaces: PurchaseModel[] = [];
  parchacesByGift: GiftPurchasersDTO[] = [];
  PurchaseFilter: PurchaseFilter = { sortBy: undefined };

  getAllparchaces(filter: PurchaseFilter): Observable<PurchaseModel[]> {
    let url = this.BASE_URL;
    if (filter.sortBy) {
      url = `${this.BASE_URL}?sortBy=${filter.sortBy}`;
    }
    return this.http.get<PurchaseModel[]>(url).pipe(
      tap({
        next: (items) => this.parchaces = items,
        error: () => this.parchaces = []
      })
    );
  }

  getPurchasesByGift(giftId: number): Observable<GiftPurchasersDTO[]> {
    return this.http.get<GiftPurchasersDTO[]>(`${this.BASE_URL}/gift/${giftId}`).pipe(
      tap({
        next: (items) => this.parchacesByGift = items,
        error: () => this.parchacesByGift = []
      })
    );
  }
  getMyPurchases(): Observable<PurchaseModel[]> {
    return this.http.get<PurchaseModel[]>(`${this.BASE_URL}/my-purchases`).pipe(
      tap({
        next: (items) => this.parchaces = items,
        error: () => this.parchaces = []
      })
    );
  }

  generateSalesReport(): Observable<Blob> {
    return this.http.get(`${this.BASE_URL}/report`, {
      responseType: 'blob'
    });
  }

}
