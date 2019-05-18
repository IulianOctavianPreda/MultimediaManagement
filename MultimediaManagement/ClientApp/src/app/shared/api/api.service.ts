import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: "root"
})
export class ApiService {
  constructor(public http: HttpClient) {}
  public getData(url) {
    return this.http.get(url);
  }
  public postData(url, data) {
    return this.http.post(url, data);
  }
  public putData(url, data) {
    return this.http.put(url, data);
  }
  public deleteData(url) {
    return this.http.delete(url);
  }
}
