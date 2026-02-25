import { Component, inject } from '@angular/core';
import { DonorService } from '../../../Services/donor/donor-service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DonorFilter, DonorModel } from '../../../Models/donor.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';


@Component({
  selector: 'app-donor',
  imports: [FormsModule, CommonModule, TableModule, ButtonModule, DialogModule, InputTextModule, ToolbarModule, ConfirmDialogModule, ToastModule, TooltipModule],
  templateUrl: './Donor.html',
  styleUrls: ['./Donor.scss'],
})
export class Donor {
  donorSrv: DonorService = inject(DonorService);
  messageService: MessageService = inject(MessageService);
  confirmationService: ConfirmationService = inject(ConfirmationService);

  serchId: number = 0;
  donor: DonorModel = {
    id: 0,
    identityNumber: '',
    firstName: '',
    lastName: '',
    city: '',
    email: '',
    phone: '',
  }

  donors: DonorModel[] = [];

  displayDialog: boolean = false;

  dialogHeader: string = 'הוספת תורם';

  myFilter: DonorFilter = {
    name: '',
    email: '',
    giftname: ''
  };

  ngOnInit() {
    this.getAll();
  }
  getEmptyDonor(): DonorModel {
    return {
      id: 0,
      identityNumber: '',
      firstName: '',
      lastName: '',
      city: '',
      email: '',
      phone: '',
    };
  }
  getAll() {
    this.donorSrv.getAll(this.myFilter).subscribe({
      next: (res) => {
        this.donors = res;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
  openNew() {
    this.donor = this.getEmptyDonor();
    this.dialogHeader = 'הוספת תורם';
    this.displayDialog = true;
  }
  //פתיחת דיאלוג עריכה
  editDonor(donor: DonorModel) {
    this.donor = { ...donor };
    this.dialogHeader = 'עריכת תורם';
    this.displayDialog = true;
  }
  //שמירת תורם חדש או ערוך
  saveDonor() {
    if (this.donor.id && this.donor.id > 0) {
      this.upDate(this.donor);
    } else {
      this.post();
    }
    this.displayDialog = false;
  }
  post() {
    this.donorSrv.post(this.donor).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'נוסף בהצלחה' });
        this.getAll();
        this.displayDialog = false;
      },
      error: (err) => {
        console.error('Server error:', err);
        const serverMessage = err.error?.message || 'אירעה שגיאה בעת שמירת התורם';

        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: serverMessage
        });
      }
    });
  }
  upDate(donor: DonorModel) {
    this.donorSrv.update(donor).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'עודכן בהצלחה' });
        this.getAll();
      },
      error: (err) => {
        console.error(err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'אירעה שגיאה בעת עדכון התורם' });
      }
    });
  }
  getById() {
    this.donorSrv.getById(this.serchId).subscribe({
      next: (res) => {
        this.donors = [res];
      },
      error: (err) => {
        console.error(err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'אירעה שגיאה בעת חיפוש התורם' });
      }
    });
  }
  delete(id: number) {
    this.confirmationService.confirm({
      message: 'האם אתה בטוח שברצונך למחוק את התורם?',
      header: 'אישור מחיקה',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'כן',
      rejectLabel: 'לא',
      accept: () => {
        this.donorSrv.delete(id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'הצלחה', detail: 'תורם נמחק בהצלחה' });
            this.getAll();
          },
          error: (err) => {
            console.error(err);
            this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: err.error?.message ?? 'המחיקה נכשלה' });
          },
        });
      },
    });
  }
  clearSearch() {
    this.serchId = 0;
    this.myFilter = { name: '', email: '', giftname: '' };
    this.getAll();
  }
}