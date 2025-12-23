import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TransactionSuccessComponent } from './transaction-success.component';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('TransactionSuccessComponent', () => {
    let component: TransactionSuccessComponent;
    let fixture: ComponentFixture<TransactionSuccessComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TransactionSuccessComponent, NoopAnimationsModule],
            providers: [provideHttpClient(), provideRouter([])]
        })
            .compileComponents();

        fixture = TestBed.createComponent(TransactionSuccessComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
