import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [CommonModule, MatToolbarModule, MatButtonModule, MatIconModule, RouterModule],
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
    userName: string = '';

    constructor(private authService: AuthService, private router: Router) { }

    ngOnInit(): void {
        const user = this.authService.currentUser;
        if (user && user.name) {
            this.userName = user.name;
        }
    }

    logout() {
        this.authService.logout();
        this.router.navigate(['/login']);
    }
}
