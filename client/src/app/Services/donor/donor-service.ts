import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DonorFilter, DonorModel } from '../../Models/donor.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DonorService {

Http:HttpClient = inject(HttpClient)
BASE_URL:string= 'http://localhost:8080/api/Donor'

getAll(filter:DonorFilter): Observable<DonorModel[]>{
  let params = new HttpParams();
  
  if(filter.name && filter.name.trim())
    params = params.set('name', filter.name.trim());

  if(filter.email && filter.email.trim())
    params = params.set('email', filter.email.trim());

  if(filter.giftname && filter.giftname.trim())
    params = params.set('giftname', filter.giftname.trim());

  return this.Http.get<DonorModel[]>(this.BASE_URL, { params });
}
post(donor:DonorModel):Observable<{ message: string }>{
  return this.Http.post<{ message: string }>(this.BASE_URL ,donor)
}
update(donor:DonorModel):Observable<DonorModel>{
  return this.Http.put<DonorModel>(`${this.BASE_URL}/${donor.id}` ,donor) 
}
getById(id:number):Observable<DonorModel>{
  return this.Http.get<DonorModel>(this.BASE_URL + '/'+ id) 
}
delete(id: number):Observable<void>{
  return this.Http.delete<void>(`${this.BASE_URL}/${id}`) 
}

}
