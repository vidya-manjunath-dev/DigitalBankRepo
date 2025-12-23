import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AccountService } from '../../services/account.service';
import { of } from 'rxjs';

describe('AdminDashboardComponent', () => {
    let component: AdminDashboardComponent;
    let fixture: ComponentFixture<AdminDashboardComponent>;
    let accountServiceSpy: jasmine.SpyObj<AccountService>;

    beforeEach(async () => {
        const spy = jasmine.createSpyObj('AccountService', ['getPendingCustomers', 'getHighValueTransactions', 'getAllServiceRequests', 'getAllAdminAccounts', 'approveCustomer', 'rejectCustomer', 'deleteAccount']);
        // Setup default returns
        spy.getPendingCustomers.and.returnValue(of([]));
        spy.getHighValueTransactions.and.returnValue(of([]));
        spy.getAllServiceRequests.and.returnValue(of([]));
        spy.getAllAdminAccounts.and.returnValue(of([]));
        spy.approveCustomer.and.returnValue(of({}));
        spy.rejectCustomer.and.returnValue(of({}));
        spy.deleteAccount.and.returnValue(of({}));

        await TestBed.configureTestingModule({
            imports: [AdminDashboardComponent, NoopAnimationsModule],
            providers: [
                provideHttpClient(),
                provideRouter([]),
                { provide: AccountService, useValue: spy }
            ]
        })
            .compileComponents();

        accountServiceSpy = TestBed.inject(AccountService) as jasmine.SpyObj<AccountService>;
        fixture = TestBed.createComponent(AdminDashboardComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load data on init', () => {
        expect(accountServiceSpy.getPendingCustomers).toHaveBeenCalled();
        expect(accountServiceSpy.getHighValueTransactions).toHaveBeenCalled();
        expect(accountServiceSpy.getAllServiceRequests).toHaveBeenCalled();
        expect(accountServiceSpy.getAllAdminAccounts).toHaveBeenCalled();
    });

    it('should call approveCustomer', () => {
        component.approveCustomer(1);
        expect(accountServiceSpy.approveCustomer).toHaveBeenCalledWith(1);
    });

    it('should call deleteAccount when confirmed', () => {
        spyOn(window, 'confirm').and.returnValue(true);
        component.deleteAccount([1]);
        expect(accountServiceSpy.deleteAccount).toHaveBeenCalledWith(1);
    });
});
