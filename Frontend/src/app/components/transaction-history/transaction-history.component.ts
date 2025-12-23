import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../services/account.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
    selector: 'app-transaction-history',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, RouterModule, NavbarComponent],
    templateUrl: './transaction-history.component.html',
    styleUrls: ['./transaction-history.component.scss']
})
export class TransactionHistoryComponent implements OnInit {
    transactions: any[] = [];
    isMiniStatement: boolean = false;
    displayedColumns: string[] = ['date', 'account', 'description', 'type', 'amount', 'balance'];

    constructor(private accountService: AccountService) { }

    ngOnInit(): void {
        this.loadTransactions();
    }

    loadTransactions(): void {
        const obs = this.isMiniStatement ?
            this.accountService.getMiniStatement() :
            this.accountService.getAllTransactions();

        obs.subscribe({
            next: (res) => this.transactions = res,
            error: (err) => console.error(err)
        });
    }

    toggleStatementView(): void {
        this.isMiniStatement = !this.isMiniStatement;
        this.loadTransactions();
    }

    downloadStatement(): void {
        this.accountService.downloadStatement().subscribe({
            next: (blob) => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `Statement_${new Date().toISOString().slice(0, 10)}.csv`;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
            },
            error: (err) => console.error(err)
        });
    }
}
