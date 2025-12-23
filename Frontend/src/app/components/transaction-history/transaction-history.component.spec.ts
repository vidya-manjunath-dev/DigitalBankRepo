import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TransactionHistoryComponent } from './transaction-history.component';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AccountService } from '../../services/account.service';
import { of } from 'rxjs';

describe('TransactionHistoryComponent', () => {
    let component: TransactionHistoryComponent;
    let fixture: ComponentFixture<TransactionHistoryComponent>;
    let accountServiceSpy: jasmine.SpyObj<AccountService>;

    beforeEach(async () => {
        const spy = jasmine.createSpyObj('AccountService', ['getAllTransactions']);
        spy.getAllTransactions.and.returnValue(of([{ id: 1, amount: 100, type: 'Credit' }]));

        await TestBed.configureTestingModule({
            imports: [TransactionHistoryComponent, NoopAnimationsModule],
            providers: [
                provideHttpClient(),
                provideRouter([]),
                { provide: AccountService, useValue: spy }
            ]
        })
            .compileComponents();

        accountServiceSpy = TestBed.inject(AccountService) as jasmine.SpyObj<AccountService>;
        fixture = TestBed.createComponent(TransactionHistoryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load transactions on init', () => {
        expect(accountServiceSpy.getAllTransactions).toHaveBeenCalled();
        expect(component.transactions.length).toBe(1);
    });
});
