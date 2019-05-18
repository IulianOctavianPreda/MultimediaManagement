import { APP_BASE_HREF } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ModalModule } from 'ngx-bootstrap/modal';
import { SpeechModule } from 'ngx-speech';
import { ToastrModule } from 'ngx-toastr';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './core/app.component';
import {
  CollectionOfMultimediaAlbumsModule,
} from './features/collection-of-multimedia-albums/collection-of-multimedia-albums.module';
import { EntryModule } from './features/entry/entry.module';
import { MultimediaAlbumModule } from './features/multimedia-album/multimedia-album.module';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    SpeechModule,
    EntryModule,
    MultimediaAlbumModule,
    CollectionOfMultimediaAlbumsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({ positionClass: "toast-top-right", preventDuplicates: true }),
    ModalModule.forRoot()
  ],
  providers: [
    { provide: APP_BASE_HREF, useValue: "/" },
    { provide: "SPEECH_LANG", useValue: "en-US" }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
