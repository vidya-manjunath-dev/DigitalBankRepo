import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private apiUrl = 'http://localhost:5105/api';

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getHeaders(): HttpHeaders {
    const token = this.authService.token;
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getAccounts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts`, { headers: this.getHeaders() });
  }

  getTransactions(accountId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts/${accountId}/transactions`, { headers: this.getHeaders() });
  }

  getAllTransactions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts/transactions`, { headers: this.getHeaders() });
  }

  getMiniStatement(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/accounts/mini-statement`, { headers: this.getHeaders() });
  }

  downloadStatement(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/accounts/download-statement`, {
      headers: this.getHeaders(),
      responseType: 'blob'
    });
  }

  deposit(accountId: number, amount: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/accounts/${accountId}/deposit`, amount, { headers: this.getHeaders().set('Content-Type', 'application/json') });
  }

  transfer(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/transfers`, data, { headers: this.getHeaders() });
  }

  createServiceRequest(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/servicerequests`, data, { headers: this.getHeaders() });
  }

  getServiceRequests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/servicerequests`, { headers: this.getHeaders() });
  }

  // Admin Methods
  getPendingCustomers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/customers/pending`, { headers: this.getHeaders() });
  }

  approveCustomer(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/customers/${id}/approve`, {}, { headers: this.getHeaders() });
  }

  rejectCustomer(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/customers/${id}/reject`, {}, { headers: this.getHeaders() });
  }

  getAllAdminAccounts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/accounts`, { headers: this.getHeaders() });
  }

  deleteAccount(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/accounts/${id}`, { headers: this.getHeaders() });
  }

  getHighValueTransactions(threshold: number = 100000): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/transactions/highvalue?threshold=${threshold}`, { headers: this.getHeaders() });
  }

  getAllServiceRequests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/servicerequests`, { headers: this.getHeaders() });
  }

  updateServiceRequestStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/servicerequests/${id}`, { status }, { headers: this.getHeaders() });
  }

  deleteServiceRequest(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/servicerequests/${id}`, { headers: this.getHeaders() });
  }
}
