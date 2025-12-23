import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../services/account.service';
import { AuthService } from '../../services/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';

import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, FormsModule, MatTableModule, RouterModule, NavbarComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  accounts: any[] = [];
  selectedAccountTransactions: any[] = [];
  displayedColumns: string[] = ['date', 'description', 'type', 'amount', 'balance'];
  userName: string = '';

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const user = this.authService.currentUser;
    if (user) {
      this.userName = user.name;
    }
    this.loadAccounts();
  }

  loadAccounts() {
    this.accountService.getAccounts().subscribe({
      next: (res) => {
        this.accounts = res;
        if (this.accounts.length > 0) {
          this.loadTransactions(this.accounts[0].id);
        }
      },
      error: (err) => console.error(err)
    });
  }

  loadTransactions(accountId: number) {
    this.accountService.getTransactions(accountId).subscribe({
      next: (res) => this.selectedAccountTransactions = res,
      error: (err) => console.error(err)
    });
  }

  logout() {
    this.authService.logout();
  }

  depositAmount: number = 0;
  selectedAccountForDeposit: any = null;

  openDeposit(account: any) {
    this.selectedAccountForDeposit = account;
    this.depositAmount = 0;
  }

  submitDeposit() {
    if (this.selectedAccountForDeposit && this.depositAmount > 0) {
      this.accountService.deposit(this.selectedAccountForDeposit.id, this.depositAmount).subscribe({
        next: (res) => {
          alert('Deposit Successful!');
          this.loadAccounts(); // Refresh
          this.selectedAccountForDeposit = null;
          this.depositAmount = 0;
        },
        error: (err) => alert('Deposit Failed: ' + (err.error?.message || err.statusText))
      });
    }
  }

  cancelDeposit() {
    this.selectedAccountForDeposit = null;
    this.depositAmount = 0;
  }
}
