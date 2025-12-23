import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    {
        path: 'dashboard',
        loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
        canActivate: [authGuard]
    },
    {
        path: 'transfer',
        loadComponent: () => import('./components/transfer/transfer.component').then(m => m.TransferComponent),
        canActivate: [authGuard]
    },
    {
        path: 'support',
        loadComponent: () => import('./components/service-request/service-request.component').then(m => m.ServiceRequestComponent),
        canActivate: [authGuard]
    },
    {
        path: 'admin',
        loadComponent: () => import('./components/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
        canActivate: [authGuard]
    },
    {
        path: 'transaction-success',
        loadComponent: () => import('./components/transaction-success/transaction-success.component').then(m => m.TransactionSuccessComponent),
        canActivate: [authGuard]
    },
    {
        path: 'transactions',
        loadComponent: () => import('./components/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
        canActivate: [authGuard]
    },
    {
        path: 'profile',
        loadComponent: () => import('./components/profile/profile.component').then(m => m.ProfileComponent),
        canActivate: [authGuard]
    }
];
