import { Routes } from '@angular/router';
import { Register } from './Components/Feature/Auth/register/register';
import { Login } from './Components/Feature/Auth/login/login';
import { GiftList } from './Components/Feature/Gift/gift-list/gift-list';
import { Category } from './Components/Feature/Category/category/category';
import { Home } from './Components/home/home';
import { Donor } from './Components/Feature/Donor/Donor';
import { GiftManage } from './Components/Feature/Gift/gift-manage/gift-manage';
import { Cart } from './Components/Feature/Ticket/cart/cart';
import { Purchases } from './Components/Feature/Ticket/purchases/purchases';
import { adminGuard } from './Guards/admin.guard';
import { Raffele } from './Components/Feature/Gift/raffele/raffele';
import { MyParchace } from './Components/Feature/Ticket/my-parchace/my-parchace';


export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },

    { path: 'home', component: Home },

    { path: 'login', component: Login },

    { path: 'register', component: Register },

    { path: 'gifts', component: GiftList },

    { path: 'giftManagement', component: GiftManage },

    { path: 'categories', component: Category },

    { path: 'donors', component: Donor },
    
     {path:'my-purchases', component:MyParchace},

    { path: 'cart', component: Cart },

    { path: 'raffle', component: Raffele, canActivate: [adminGuard] },

    { path: 'purchases', component: Purchases, canActivate: [adminGuard] },

];