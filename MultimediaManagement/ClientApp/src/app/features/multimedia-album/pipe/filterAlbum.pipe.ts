import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: "filterAlbum"
})
export class FilterAlbumPipe implements PipeTransform {
  transform(items: any[], searchText: string): any[] {
    if (!items) return [];
    if (!searchText) return items;

    searchText = searchText.toLowerCase();
    return items.filter((item) => {
      return (
        item.name.toLowerCase().includes(searchText) ||
        item.extension.toLowerCase().includes(searchText) ||
        item.keywords.split(",").some((word) => {
          return word.toLowerCase().includes(searchText);
        })
      );
    });
  }
}
