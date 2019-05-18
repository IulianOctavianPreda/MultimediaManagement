import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {
  CollectionOfMultimediaAlbumsComponent,
} from './features/collection-of-multimedia-albums/collection-of-multimedia-albums.component';
import { EntryComponent } from './features/entry/entry.component';
import { MultimediaAlbumComponent } from './features/multimedia-album/multimedia-album.component';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
  {
    path: "",
    component: EntryComponent
  },
  {
    path: "collections",
    component: CollectionOfMultimediaAlbumsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: ":id",
    component: MultimediaAlbumComponent,
    canActivate: [AuthGuard]
  },
  {
    path: "**",
    redirectTo: ""
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
