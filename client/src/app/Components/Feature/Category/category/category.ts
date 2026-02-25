import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../../../Services/category/category-service';
import { CategoryModel } from '../../../../Models/category.model';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DataViewModule } from 'primeng/dataview';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [FormsModule, CommonModule, TableModule, DialogModule, ButtonModule, InputTextModule, ToastModule, DataViewModule],
  templateUrl: './category.html',
  styleUrls: ['./category.scss'],
})
export class Category implements OnInit {

  private categorySrv = inject(CategoryService);
  private messageService = inject(MessageService);
  categories: CategoryModel[] = [];


  newCategoryName: string = '';
  displayDialog: boolean = false;

  isAdmin: boolean = false;

  ngOnInit() {
    this.checkUserRole();
    this.getAll();
  }


  checkUserRole() {
    const role = localStorage.getItem('role');
    if (role === 'Admin' || role === 'Manager') {
      this.isAdmin = true;
    } else {
      this.isAdmin = false;
    }
  }

  getAll() {
    this.categorySrv.getAll().subscribe({
      next: (data) => {
        console.log("הנתונים שהתקבלו:", data);
        this.categories = data;
      },
      error: (err) => {
        console.error("שגיאה בטעינת קטגוריות:", err);
        this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'לא ניתן לטעון נתונים' });
      }
    });
  }

  showAddDialog() {
    this.newCategoryName = '';
    this.displayDialog = true;
  }

  addCategory() {
    if (!this.newCategoryName || this.newCategoryName.trim() === '') {
      this.messageService.add({ severity: 'warn', summary: 'שים לב', detail: 'חובה להזין שם לקטגוריה' });
      return;
    }

    const categoryToSend: CategoryModel = {
      id: 0,
      name: this.newCategoryName
    };


    this.categorySrv.post(categoryToSend).subscribe({
      next: (data) => {
        console.log("נוסף בהצלחה:", data);

        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה',
          detail: 'הקטגוריה נוספה בהצלחה!'
        });

        this.getAll();
        this.displayDialog = false;
      },
      error: (err) => {
        console.error("שגיאה בהוספה:", err);
        const errorMsg = err.error ? err.error : 'אירעה שגיאה בשמירת הנתונים';

        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: errorMsg
        });
      }
    });
  }

 deleteCategory(id: number) {
  this.categorySrv.delete(id).subscribe({
    next: (data) => {
      this.messageService.add({
        severity: 'success',
        summary: 'הצלחה',
        detail: 'הקטגוריה נמחקה בהצלחה!'
      });
      this.getAll(); 
    },
    error: (err) => {
      console.error("פרטי השגיאה:", err);

      let msg = 'אירעה שגיאה במחיקת הקטגוריה';
      if (err.error && err.error.message) {
        msg = err.error.message;
      }
      else if (typeof err.error === 'string') {
        msg = err.error;
      }

      this.messageService.add({
        severity: 'error',
        summary: 'שגיאה',
        detail: msg
      });
    }
  });
}
}