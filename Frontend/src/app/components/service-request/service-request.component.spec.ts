import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ServiceRequestComponent } from './service-request.component';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AccountService } from '../../services/account.service';
import { of } from 'rxjs';

import { AuthService } from '../../services/auth.service';

describe('ServiceRequestComponent', () => {
    let component: ServiceRequestComponent;
    let fixture: ComponentFixture<ServiceRequestComponent>;
    let accountServiceSpy: jasmine.SpyObj<AccountService>;
    let authServiceMock: any;

    beforeEach(async () => {
        accountServiceSpy = jasmine.createSpyObj('AccountService', ['getServiceRequests', 'createServiceRequest']);
        // Mock getServiceRequests to return an empty array by default
        accountServiceSpy.getServiceRequests.and.returnValue(of([]));

        authServiceMock = {
            get currentUser() { return { name: 'Test User', role: 'Customer' }; },
            get isAuthenticated() { return true; },
            logout: jasmine.createSpy('logout')
        };

        await TestBed.configureTestingModule({
            imports: [ServiceRequestComponent, NoopAnimationsModule],
            providers: [
                { provide: AccountService, useValue: accountServiceSpy },
                { provide: AuthService, useValue: authServiceMock },
                provideRouter([])
            ]
        })
            .compileComponents();

        fixture = TestBed.createComponent(ServiceRequestComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
