import { Component, OnInit } from '@angular/core';
import { DataManagerService } from 'src/app/shared/services/data-manager.service';

@Component({
  selector: "app-entry",
  templateUrl: "./entry.component.html",
  styleUrls: ["./entry.component.scss"]
})
export class EntryComponent implements OnInit {
  selected = "login";

  constructor(public dataManager: DataManagerService) {}

  ngOnInit() {
    this.dataManager.setState("entry");
  }

  changeSelected(value) {
    this.selected = value;
  }
}
