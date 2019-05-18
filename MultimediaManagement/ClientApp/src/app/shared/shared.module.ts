import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AudioModuleComponent } from './modularComponents/audio-module/audio-module.component';
import { ImageModuleComponent } from './modularComponents/image-module/image-module.component';
import { PdfModuleComponent } from './modularComponents/pdf-module/pdf-module.component';
import { SvgModuleComponent } from './modularComponents/svg-module/svg-module.component';
import { VideoModuleComponent } from './modularComponents/video-module/video-module.component';
import { YoutubeModuleComponent } from './modularComponents/youtube-module/youtube-module.component';

@NgModule({
  declarations: [
    PdfModuleComponent,
    SvgModuleComponent,
    YoutubeModuleComponent,
    ImageModuleComponent,
    VideoModuleComponent,
    AudioModuleComponent
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PdfModuleComponent,
    SvgModuleComponent,
    YoutubeModuleComponent,
    ImageModuleComponent,
    VideoModuleComponent,
    AudioModuleComponent
  ]
})
export class SharedModule {}
