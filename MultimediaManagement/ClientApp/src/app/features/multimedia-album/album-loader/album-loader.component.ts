import { Component, Input, OnInit } from '@angular/core';
import { ApiService } from 'src/app/shared/api/api.service';

@Component({
  selector: "app-album-loader",
  templateUrl: "./album-loader.component.html",
  styleUrls: ["./album-loader.component.scss"]
})
export class AlbumLoaderComponent implements OnInit {
  @Input()
  sourceUrl;
  @Input()
  entityId;

  source;
  extension;

  constructor(public api: ApiService) {}

  ngOnInit() {
    this.getEntity(this.sourceUrl.replace("/$entityId", `/${this.entityId}`));
  }

  getEntity(entityUrl) {
    this.api.getData(entityUrl).subscribe((data: any) => {
      this.source = data.data;
      this.extension = data.extension;
    });
  }
}
