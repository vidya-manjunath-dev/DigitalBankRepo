import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, RouterModule, NavbarComponent],
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    profile: any = null;
    loading: boolean = true;
    error: string | null = null;

    constructor(private authService: AuthService) { }

    ngOnInit(): void {
        this.authService.getProfile().subscribe({
            next: (res) => {
                this.profile = res;
                this.loading = false;
            },
            error: (err) => {
                console.error(err);
                this.error = err.error?.message || 'Failed to load profile details.';
                this.loading = false;
            }
        });
    }
}
