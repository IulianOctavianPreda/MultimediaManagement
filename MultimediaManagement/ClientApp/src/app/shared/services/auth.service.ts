import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { DataManagerService } from './data-manager.service';

@Injectable({
  providedIn: "root"
})
export class AuthService {
  constructor(public dataManager: DataManagerService, public http: HttpClient) {}

  isAuthenticated() {
    return this.http.post("http://localhost:49882/api/Security/searchToken", {
      token: this.dataManager.token
    });
  }
}
