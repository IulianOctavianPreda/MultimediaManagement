import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: "filterCollections"
})
export class FilterCollectionsPipe implements PipeTransform {
  transform(items: any[], searchText: string): any[] {
    if (!items) return [];
    if (!searchText) return items;

    searchText = searchText.toLowerCase();
    return items.filter((item) => {
      var type = item.type;
      if (type == 0) {
        type = "public";
      } else {
        type = "private";
      }
      return (
        item.name.toLowerCase().includes(searchText) ||
        item.keywords.split(",").some((word) => {
          return word.toLowerCase().includes(searchText);
        }) ||
        type.includes(searchText)
      );
    });
  }
}
