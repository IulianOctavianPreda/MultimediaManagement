import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: "root"
})
export class FileDownloadService {
  constructor(private http: HttpClient) {}

  downloadFile(url, body, fileName): any {
    this.http.post(url, body, { responseType: "arraybuffer" }).subscribe((response) => {
      var blob = new Blob([response], { type: "application/octet-stream" });
      saveAs(blob, fileName);
    });
  }
}
