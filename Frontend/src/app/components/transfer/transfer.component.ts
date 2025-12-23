import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-transfer',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    RouterModule,
    NavbarComponent
  ],
  templateUrl: './transfer.component.html',
  styleUrl: './transfer.component.scss'
})
export class TransferComponent implements OnInit {
  transferForm: FormGroup;
  accounts: any[] = [];
  successMessage: string = '';
  error: string = '';

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router
  ) {
    this.transferForm = this.fb.group({
      fromAccountId: ['', Validators.required],
      toAccountId: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(1)]],
      remarks: ['']
    });
  }

  ngOnInit(): void {
    this.accountService.getAccounts().subscribe({
      next: (res) => this.accounts = res,
      error: (err) => console.error(err)
    });
  }

  onSubmit() {
    if (this.transferForm.valid) {
      if (this.transferForm.value.fromAccountId === this.transferForm.value.toAccountId) {
        this.error = "Cannot transfer to the same account.";
        return;
      }

      this.accountService.transfer(this.transferForm.value).subscribe({
        next: (res) => {
          const fromAccount = this.accounts.find(a => a.id === this.transferForm.value.fromAccountId);
          const toAccount = this.accounts.find(a => a.id === this.transferForm.value.toAccountId);

          this.router.navigate(['/transaction-success'], {
            state: {
              data: {
                amount: this.transferForm.value.amount,
                fromAccount: fromAccount ? `${fromAccount.accountType} - ${fromAccount.accountNumber}` : 'Unknown',
                toAccount: toAccount ? `${toAccount.accountType} - ${toAccount.accountNumber}` : 'Unknown',
                date: new Date(),
                newBalance: res.newBalance
              }
            }
          });
        },
        error: (err) => {
          this.error = err.error?.message || 'Transfer failed';
          this.successMessage = '';
        }
      });
    }
  }
}
