import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../services/account.service';
import { AuthService } from '../../services/auth.service';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, MatTabsModule, MatTableModule, MatButtonModule, MatCardModule, MatIconModule, MatToolbarModule, MatSelectModule, MatFormFieldModule, MatTooltipModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  pendingCustomers: any[] = [];
  highValueTransactions: any[] = [];
  serviceRequests: any[] = [];
  customerAccounts: any[] = [];

  displayedColumnsCustomers: string[] = ['name', 'email', 'date', 'actions'];
  displayedColumnsTxns: string[] = ['date', 'amount', 'type', 'account'];
  displayedColumnsRequests: string[] = ['date', 'title', 'customer', 'status', 'action'];
  displayedColumnsAccounts: string[] = ['customer', 'accNumbers', 'totalBalance'];

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.accountService.getPendingCustomers().subscribe({
      next: (res) => this.pendingCustomers = res,
      error: (err) => console.error(err)
    });

    this.accountService.getHighValueTransactions().subscribe({
      next: (res) => this.highValueTransactions = res,
      error: (err) => console.error(err)
    });

    this.accountService.getAllServiceRequests().subscribe({
      next: (res) => this.serviceRequests = res,
      error: (err) => console.error(err)
    });

    this.loadAccounts();
  }

  approveCustomer(id: number) {
    this.accountService.approveCustomer(id).subscribe({
      next: () => this.loadData(),
      error: (err) => console.error(err)
    });
  }

  rejectCustomer(id: number) {
    if (confirm('Are you sure you want to REJECT this customer?')) {
      this.accountService.rejectCustomer(id).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error(err)
      });
    }
  }

  loadAccounts() {
    this.accountService.getAllAdminAccounts().subscribe({
      next: (res) => {
        if (!res || !Array.isArray(res)) {
          this.customerAccounts = [];
          return;
        }
        const grouped = res.reduce((acc: any, curr: any) => {
          const key = curr.customerId || curr.CustomerId;
          if (!key) return acc;

          if (!acc[key]) {
            acc[key] = {
              customerId: key,
              customerName: (curr.customer?.name || curr.Customer?.name || 'Unknown'),
              accountIds: [],
              accountNumbers: [],
              totalBalance: 0,
              isBlocked: true
            };
          }
          acc[key].accountIds.push(curr.id || curr.Id);
          acc[key].accountNumbers.push(curr.accountNumber || curr.AccountNumber);
          acc[key].totalBalance += (curr.balance || curr.Balance || 0);

          const status = curr.status || curr.Status;
          if (status === 'Active') {
            acc[key].isBlocked = false;
          }
          return acc;
        }, {});
        this.customerAccounts = Object.values(grouped);
      },
      error: (err) => {
        console.error('Error loading accounts:', err);
        this.customerAccounts = [];
      }
    });
  }

  deleteAccount(ids: number[]) {
    if (confirm('Are you sure you want to DELETE all accounts for this customer? This cannot be undone.')) {
      const deleteNext = (index: number) => {
        if (index >= ids.length) {
          this.loadAccounts();
          return;
        }
        this.accountService.deleteAccount(ids[index]).subscribe({
          next: () => deleteNext(index + 1),
          error: (err) => {
            alert(err.error.message || 'Error deleting account');
            this.loadAccounts();
          }
        });
      };
      deleteNext(0);
    }
  }

  updateRequestStatus(id: number, status: string) {
    this.accountService.updateServiceRequestStatus(id, status).subscribe({
      next: () => this.loadData(),
      error: (err) => console.error(err)
    });
  }

  deleteRequest(id: number) {
    if (confirm('Are you sure you want to delete this closed request?')) {
      this.accountService.deleteServiceRequest(id).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error(err)
      });
    }
  }

  logout() {
    this.authService.logout();
  }
}
