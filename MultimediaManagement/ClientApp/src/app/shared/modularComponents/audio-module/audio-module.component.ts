import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: "app-audio-module",
  templateUrl: "./audio-module.component.html",
  styleUrls: ["./audio-module.component.scss"]
})
export class AudioModuleComponent implements OnInit {
  @Input()
  source: any;

  constructor(public sanitizer: DomSanitizer) {}

  ngOnInit() {}
}
