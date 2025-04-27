import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from './user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = 'https://localhost:7100/UserManagement'; // Replace with your API URL

  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + "/GetUsersList");
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl+ "/GetUser?id="+ id);
  }
}