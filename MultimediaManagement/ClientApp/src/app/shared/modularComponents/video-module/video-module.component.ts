import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: "app-video-module",
  templateUrl: "./video-module.component.html",
  styleUrls: ["./video-module.component.scss"]
})
export class VideoModuleComponent implements OnInit {
  @Input()
  source: any;
  constructor(public sanitizer: DomSanitizer) {}

  ngOnInit() {}
}
