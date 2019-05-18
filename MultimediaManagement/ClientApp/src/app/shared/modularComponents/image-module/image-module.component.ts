import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: "app-image-module",
  templateUrl: "./image-module.component.html",
  styleUrls: ["./image-module.component.scss"]
})
export class ImageModuleComponent implements OnInit {
  @Input()
  source: any;

  constructor(public sanitizer: DomSanitizer) {}

  ngOnInit() {}
}
