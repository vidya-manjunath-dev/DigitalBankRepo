import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';

import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-service-request',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatInputModule, MatButtonModule, MatTableModule, RouterModule, NavbarComponent],
  templateUrl: './service-request.component.html',
  styleUrl: './service-request.component.scss'
})
export class ServiceRequestComponent implements OnInit {
  requestForm: FormGroup;
  requests: any[] = [];
  displayedColumns: string[] = ['date', 'title', 'status'];
  successMessage: string = '';

  constructor(private fb: FormBuilder, private accountService: AccountService) {
    this.requestForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests() {
    this.accountService.getServiceRequests().subscribe({
      next: (res) => this.requests = res,
      error: (err) => console.error(err)
    });
  }

  onSubmit() {
    if (this.requestForm.valid) {
      this.accountService.createServiceRequest(this.requestForm.value).subscribe({
        next: (res) => {
          this.successMessage = 'Request submitted successfully.';
          this.requestForm.reset();
          this.loadRequests();
        },
        error: (err) => console.error(err)
      });
    }
  }
}
