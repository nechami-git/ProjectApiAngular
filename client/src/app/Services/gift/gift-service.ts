
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GiftModel, GiftFilter, GiftFilterAdmin } from '../../Models/gift.model'; 

@Injectable({
  providedIn: 'root'
})
export class GiftService {

  private BASE_URL = 'http://localhost:8080/api/Gift';

  constructor(private http: HttpClient) { }

  getAll(filter: GiftFilter = {}): Observable<GiftModel[]> {
   
    let params = new HttpParams();

    if (filter.name) {
      params = params.set('name', filter.name);
    }
    if (filter.CategoryName) {
      params = params.set('CategoryName', filter.CategoryName);
    }

    if (filter.TicketPrice) {
      params = params.set('TicketPrice', filter.TicketPrice.toString());
    }
    return this.http.get<GiftModel[]>(this.BASE_URL, { params: params });
  }

  getAdminGifts(filter: GiftFilterAdmin): Observable<GiftModel[]> {
   
    let params = new HttpParams();
    if (filter.name) {
      params = params.set('name', filter.name);
    }
    if (filter.donorName) {
      params = params.set('donorName', filter.donorName);
    }
    if (filter.purchaseCount) {
      params = params.set('purchaseCount', filter.purchaseCount.toString());
    }
    return this.http.get<GiftModel[]>(`${this.BASE_URL}/Manager`, { params: params });
  }

  postGift(gift: GiftModel): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.BASE_URL, gift);
  }
  updateGift(gift: GiftModel): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.BASE_URL}/${gift.id}`, gift);
  }
  deleteGift(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/${id}`);
  }
  getGiftById(id: number): Observable<GiftModel> {
    return this.http.get<GiftModel>(`${this.BASE_URL}/${id}`);
  }
    performRaffle(id:number): Observable<GiftModel> {
    return this.http.post<GiftModel>(`${this.BASE_URL}/Raffle/${id}`, {id:id});
  }

  generateReport(): Observable<Blob> {
    return this.http.get(`${this.BASE_URL}/report`, { responseType: 'blob' });
  }

}