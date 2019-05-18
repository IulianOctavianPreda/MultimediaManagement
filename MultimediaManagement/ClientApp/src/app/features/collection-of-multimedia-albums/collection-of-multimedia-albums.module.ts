import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';

import { CollectionOfMultimediaAlbumsComponent } from './collection-of-multimedia-albums.component';
import { FilterCollectionsPipe } from './pipe/filterCollections.pipe';

@NgModule({
  declarations: [CollectionOfMultimediaAlbumsComponent, FilterCollectionsPipe],
  imports: [SharedModule]
})
export class CollectionOfMultimediaAlbumsModule {}
