import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CategoryModel } from '../../Models/category.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CategoryService{
  private BASE_URL = 'http://localhost:8080/api/Category';

  constructor(private http: HttpClient) { }

   getAll(): Observable<CategoryModel[]> {
    return this.http.get<CategoryModel[]>(this.BASE_URL);
  }
  post(category: CategoryModel): Observable<CategoryModel[]> {
    return this.http.post<CategoryModel[]>(this.BASE_URL, category);
  }
  delete(id: number): Observable<CategoryModel[]> {
    return this.http.delete<CategoryModel[]>(`${this.BASE_URL}/${id}`);
  }
}
