import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';

import { AlbumLoaderComponent } from './album-loader/album-loader.component';
import { LightboxComponent } from './lightbox/lightbox.component';
import { MultimediaAlbumComponent } from './multimedia-album.component';
import { FilterAlbumPipe } from './pipe/filterAlbum.pipe';
import { UploadComponent } from './upload/upload.component';

@NgModule({
  declarations: [
    MultimediaAlbumComponent,
    UploadComponent,
    LightboxComponent,
    FilterAlbumPipe,
    AlbumLoaderComponent
  ],
  imports: [SharedModule]
})
export class MultimediaAlbumModule {}
