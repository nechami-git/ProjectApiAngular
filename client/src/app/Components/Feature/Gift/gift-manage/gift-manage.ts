import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DividerModule } from 'primeng/divider';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { DropdownModule } from 'primeng/dropdown'; // <--- חובה לייבא את זה!
import { ConfirmationService, MessageService } from 'primeng/api';

// Models & Services
import { GiftService } from '../../../../Services/gift/gift-service';
import { CategoryService } from '../../../../Services/category/category-service';
import { DonorService } from '../../../../Services/donor/donor-service'; // <--- שירות תורמים

import { GiftFilterAdmin, GiftModel } from '../../../../Models/gift.model';
import { CategoryModel } from '../../../../Models/category.model';
import { DonorModel } from '../../../../Models/donor.model';

@Component({
  selector: 'app-gift-manage',
  standalone: true,
  imports: [ CommonModule, ReactiveFormsModule, FormsModule,TableModule, ButtonModule, InputTextModule, InputNumberModule, InputTextareaModule, DialogModule, ToastModule, ConfirmDialogModule,  DividerModule, TagModule, TooltipModule, DropdownModule  ],
  templateUrl: './gift-manage.html',
  styleUrls: ['./gift-manage.scss'],
})
export class GiftManage {

  giftSrv = inject(GiftService);
  categorySrv = inject(CategoryService); 
  donorSrv = inject(DonorService);    
  
  messegeService = inject(MessageService);
  confirmationService = inject(ConfirmationService);

  categories: CategoryModel[] = []; 
  donors: DonorModel[] = [];

  frmGift: FormGroup = new FormGroup({});
  gifts: GiftModel[] = [];
  
  filter: GiftFilterAdmin = {
    name: '', donorName: '', purchaseCount: 0,};

  giftDialog: boolean = false;
  submitted: boolean = false;
  dialogHeader: string = '';
  gift: GiftModel = this.getEmptyGift();

  ngOnInit() {
    this.loadGifts();
    this.loadCategories(); 
    this.loadDonors();
  }

  loadCategories() {
    this.categorySrv.getAll().subscribe({
      next: (res) => this.categories = res,
      error: (err) => console.error('Error loading categories', err)
    });
  }

  loadDonors() {
    this.donorSrv.getAll({name: '', email: '', giftname: ''}).subscribe({
      next: (res) => this.donors = res,
      error: (err) => console.error('Error loading donors', err)
    });
  }
  getEmptyGift(): GiftModel {
    return {
      id: 0, name: '', description: '', image: '', ticketPrice: 0, 
      categoryId: 0, categoryName: '', donorId: 0, donorName: '', 
      price: 0, participantsCount: 0, winnerName: ''
    };
  }

  initForm(g?: GiftModel): FormGroup {
    return new FormGroup({
      id: new FormControl(g ? g.id : 0),
      name: new FormControl(g ? g.name : '', [Validators.required, Validators.minLength(3)]),
      description: new FormControl(g ? g.description : ''),
      image: new FormControl(g ? g.image : ''),
      ticketPrice: new FormControl(g ? g.ticketPrice : null, [Validators.required, Validators.min(1)]),
      price: new FormControl(g ? g.price : null, [Validators.required, Validators.min(1)]),
      
      categoryId: new FormControl(g ? g.categoryId : null, [Validators.required]),
      donorId: new FormControl(g ? g.donorId : null, [Validators.required]),
      
      categoryName: new FormControl(g ? g.categoryName : ''),
      donorName: new FormControl(g ? g.donorName : ''),
    });
  }

  loadGifts() {
    this.giftSrv.getAdminGifts(this.filter).subscribe({
        next: (res) => this.gifts = res,
        error: (err) => this.messegeService.add({severity:'error', summary:'שגיאה', detail:'תקלה בטעינת נתונים'})
    });
  }

  openNew() {
    this.gift = this.getEmptyGift();
    this.frmGift = this.initForm();
    this.dialogHeader = 'יצירת מתנה חדשה';
    this.giftDialog = true;
    this.submitted = false;
  }

  editGift(gift: GiftModel) {
    this.gift = { ...gift };
    this.frmGift = this.initForm(this.gift);
    this.dialogHeader = 'עריכת פרטי מתנה';
    this.giftDialog = true;
  }

  hideDialog() {
    this.giftDialog = false;
    this.submitted = false;
  }

  clearSearch() {
     this.filter = { name: '', donorName: '', purchaseCount: 0 };
     this.loadGifts();
  }

  saveGift() {
    this.submitted = true;
    if (this.frmGift.invalid) return;

    const formValue = this.frmGift.getRawValue();
    const giftToSave: GiftModel = {
      ...this.gift,
      ...formValue,
      price: Number(formValue.price),
      ticketPrice: Number(formValue.ticketPrice)
    };

    this.gift = giftToSave;

    if (this.gift.id && this.gift.id > 0) {
      this.updateGift();
    } else {
      this.postGift();
    }
  }

  postGift() {
    this.giftSrv.postGift(this.gift).subscribe({
      next: () => {
        this.messegeService.add({ severity: 'success', summary: 'הצלחה', detail: 'מתנה נוספה', life: 3000 });
        this.giftDialog = false;
        this.loadGifts();
      },
      error: (err) => this.messegeService.add({ severity: 'error', summary: 'שגיאה', detail: err.error?.message })
    });
  }

  updateGift() {
    this.giftSrv.updateGift(this.gift).subscribe({
      next: () => {
        this.messegeService.add({ severity: 'success', summary: 'הצלחה', detail: 'מתנה עודכנה', life: 3000 });
        this.giftDialog = false;
        this.loadGifts();
      },
      error: (err) => this.messegeService.add({ severity: 'error', summary: 'שגיאה', detail: err.error?.message })
    });
  }

  deleteGift(id: number) {
    this.confirmationService.confirm({
      message: 'למחוק את המתנה?',
      header: 'אישור מחיקה',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.giftSrv.deleteGift(id).subscribe({
          next: () => {
             this.messegeService.add({ severity: 'success', summary: 'נמחק', detail: 'המתנה הוסרה בהצלחה' });
             this.loadGifts();
          },
          error: (err) => this.messegeService.add({ severity: 'error', summary: 'שגיאה', detail: err.error?.message })
        });
      }
    });
  }
}