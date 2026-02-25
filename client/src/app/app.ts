import { Component, inject, signal } from '@angular/core';
import { RouterOutlet,RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './Services/auth/authService';

import { ButtonModule } from 'primeng/button'; 
import { ToolbarModule } from 'primeng/toolbar';   
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { SidebarModule } from 'primeng/sidebar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive, ButtonModule, ToolbarModule, MenuModule, SidebarModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App {
  protected readonly title = signal('Chinese Sale');
  authSrv:AuthService = inject(AuthService)

  adminGiftMenu: MenuItem[] | undefined;
  sidebarVisible: boolean = false;

  ngOnInit() {

    this.adminGiftMenu = [
      {
        label: 'ניהול השוטף',
        items: [
            {
                label:'הגרלת מתנות',
                icon: 'pi pi-trophy',
                routerLink: '/raffle'
            },
            {
                label: 'ניהול מתנות',
                icon: 'pi pi-box',
                routerLink: '/giftManagement' 
            }
        ]
      },

      {
        label: 'ניהול מערכת',
        items: [
            {
                label: 'ניהול תורמים',
                icon: 'pi pi-users',
                routerLink: '/donors'
            },
            {
                label: 'ניהול קטגוריות',
                icon: 'pi pi-tags',
                routerLink: '/categories'
            },
            {
                label: 'רכישות המערכת',
                icon: 'pi pi-table',
                routerLink: '/purchases'
            }
        ]
      }
    ];
  }
}

