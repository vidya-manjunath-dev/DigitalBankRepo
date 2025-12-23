import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
    selector: 'app-transaction-success',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, RouterModule, NavbarComponent],
    templateUrl: './transaction-success.component.html',
    styleUrls: ['./transaction-success.component.scss']
})
export class TransactionSuccessComponent implements OnInit {
    transactionDetails: any = null;

    constructor(private router: Router) {
        const navigation = this.router.getCurrentNavigation();
        if (navigation?.extras?.state) {
            this.transactionDetails = navigation.extras.state['data'];
        }
    }

    ngOnInit(): void {
        if (!this.transactionDetails) {
            // Fallback if accessed directly or without state
            this.router.navigate(['/dashboard']);
        }
    }

    goToDashboard() {
        this.router.navigate(['/dashboard']);
    }
}
