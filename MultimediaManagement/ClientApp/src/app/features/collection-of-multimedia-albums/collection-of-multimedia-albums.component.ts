import { Component, HostListener, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SpeechService } from 'ngx-speech';
import { EMPTY, Subject } from 'rxjs';
import { catchError, map, take, takeUntil } from 'rxjs/operators';
import { ApiService } from 'src/app/shared/api/api.service';
import { SnakeService } from 'src/app/shared/easterEgg/snake.service';
import { DataManagerService } from 'src/app/shared/services/data-manager.service';
import { FloatingMicrophoneService } from 'src/app/shared/services/floating-microphone.service';

@Component({
  selector: "app-collection-of-multimedia-albums",
  templateUrl: "./collection-of-multimedia-albums.component.html",
  styleUrls: ["./collection-of-multimedia-albums.component.scss"]
})
export class CollectionOfMultimediaAlbumsComponent implements OnInit, OnDestroy {
  /**
   * Specific options will override batch options from json object
   */
  @Input()
  gridSize: number; // number of albums displayed on a row

  @Input()
  gridSizeSuggestions: number; // number of albums displayed on a row

  // @Input()
  userId: any;

  @Input()
  collectionUrl: string; // api call to retrieve the albums in the collection it can be used for both local and remote resources like a local json or from a server

  @Input()
  suggestedCollectionUrl: string; // api call to retrieve the albums in the collection

  @Input()
  deleteCollectionUrl: string; // api call to delete the albums in the collection

  @Input()
  addCollectionUrl: string;

  @Input()
  skip: number;

  @Input()
  take: number;

  @Input()
  albumUrl: string; // api call to retrieve the albums in the Album it can be used for both local and remote resources like a local json or from a server

  @Input()
  suggestedEntityUrl: string; // api call to retrieve the Entities in the Entity

  @Input()
  deleteEntityUrl: string; // api call to delete the Entities in the Entity

  @Input()
  addEntitiesUrl: string;

  @Input()
  getEntityUrl: string;

  @Input()
  slideShow: boolean;

  @Input()
  lockSlideShow: boolean;

  @Input()
  slideShowTimeBeforeNext: number;

  @Input()
  configPath: string;

  // used to redirect to the album
  //placeholders for the images of the album(in 3d mode or image in 2d)
  @Input()
  bootstrapAccentPrimary: string;

  @Input()
  bootstrapAccentSecondary: string;

  _collectionData;
  _suggestedCollectionData;
  _searchText;
  _toggleView;

  _mostUsedKeywords;
  _loadedFirstTime = false;
  _noMoreData = false;
  _deleteAccent;
  _markedForDeletion;
  _addNewCollectionForm;
  _modalDeleteConfirmation = "";
  _scrollAmount;

  private _destroyed = new Subject<void>();
  constructor(
    public api: ApiService,
    private formBuilder: FormBuilder,
    public speech: SpeechService,
    public snake: SnakeService,
    public router: Router,
    private floatingMicrophone: FloatingMicrophoneService,
    public dataManager: DataManagerService
  ) {}

  ngOnInit() {
    this._addNewCollectionForm = this.formBuilder.group({
      collectionName: ["", Validators.required],
      collectionType: ["", Validators.required],
      keywords: ["", Validators.required]
    });
    this.loadInputOptionsOrDefault();
    this.speechActions();
    this.floatingMicrophone.makeFloatingMicrophone(this.speech);
    this.snake.snake();

    this.userId = this.dataManager.userId;
  }

  ngOnDestroy(): void {
    this._destroyed.next();
    this._destroyed.complete();
  }

  speechActions() {
    this.speech.message
      .pipe(
        catchError((err) => {
          this.toggleMic();
          return EMPTY;
        }),
        takeUntil(this._destroyed)
      )
      .subscribe((msg) => {
        console.log(msg);
        if (msg.message == "delete") {
          document.getElementById("deleteCollectionButton").click();
        }
        if (msg.message == "add") {
          document.getElementById("addCollectionButton").click();
        }
        if (
          msg.message == "suggestions" ||
          msg.message == "tip" ||
          msg.message == "recommendations"
        ) {
          document.getElementById("suggestionsCollectionButton").click();
        }
        if (msg.message == "snake") {
          this.snake.snake();
        }
        this.toggleMic();
      });
  }

  toggleMic() {
    var elem = document.getElementById("floatingMicrophone");
    if (elem.classList.contains("notRecording")) {
      elem.classList.remove("notRecording");
      elem.classList.add("recording");
      this.speech.start();
    } else {
      elem.classList.remove("recording");
      elem.classList.add("notRecording");
      this.speech.stop();
    }
  }

  getCollections() {
    this.api
      .getData(
        this.collectionUrl
          .replace("/$userId", `/${this.userId}`)
          .replace("/$take", `/${this.take}`)
          .replace("/$skip", `/${this.skip}`)
      )
      .pipe(take(1))
      .subscribe((data) => {
        if (typeof this._collectionData == "undefined") {
          this._collectionData = [];
        }
        data["collection"].forEach((collection) => {
          //console.log(collection);
          //console.log(collection["placeholder"].length);

          if (collection["placeholder"].length == 0) {
            collection["placeholder"] = [
              {
                data: "../../../assets/new-collection.jpg"
              }
            ];
          }
        });
        if (data["collection"].length != 0) {
          //console.log(this._collectionData);
          this._collectionData = [...this._collectionData, ...data["collection"]];
          this.skip += this.take;
          var str = "";
          this._collectionData.forEach((collection) => {
            str += "," + collection["keywords"];
          });
          this._mostUsedKeywords = this.mostUsedKeywords(str);
          //console.log(typeof this.suggestedCollectionUrl);
          if (typeof this.suggestedCollectionUrl != "undefined") {
            //console.log(typeof this.suggestedCollectionUrl);

            this.getSuggestedCollections();
          }
          this._loadedFirstTime = true;
        } else {
          this._noMoreData = true;
        }
      });
  }

  mostUsedKeywords(str) {
    var wordCounts = {};
    var words = str.split(",");
    for (var i = 0; i < words.length; i++) {
      wordCounts["_" + words[i]] = (wordCounts["_" + words[i]] || 0) + 1;
    }
    delete wordCounts["_"];
    var wordsList = "";
    wordCounts = JSON.parse(JSON.stringify(wordCounts).replace(/_/g, ""));
    for (var k in wordCounts) {
      wordsList += "," + k;
    }
    wordsList = wordsList.replace(",", "");
    //console.log(wordsList);
    return wordsList;
  }

  getSuggestedCollections() {
    //for suggestions we skip 0 since we want only the best suggestions
    this.api
      .getData(
        this.suggestedCollectionUrl
          .replace("/$userId", `/${this.userId}`)
          .replace("/$take", `/${this.take}`)
          .replace("/$skip", `/0`) + `/${this._mostUsedKeywords}`
      )
      .pipe(take(1))
      .subscribe((data: Array<{}>) => {
        data.forEach((collection) => {
          //console.log(collection);
          //console.log(collection["placeholder"].length);

          if (collection["placeholder"].length == 0) {
            collection["placeholder"] = [
              {
                data: "../../../assets/new-collection.jpg"
              }
            ];
          }
        });
        this._suggestedCollectionData = data;
      });
  }

  @HostListener("window:scroll", ["$event"])
  onWindowScroll() {
    //In chrome and some browser scroll is given to body tag
    let pos =
      (document.documentElement.scrollTop || document.body.scrollTop) +
      document.documentElement.offsetHeight;
    let max = document.documentElement.scrollHeight;
    //console.log(pos);
    //console.log(max);
    if (typeof this._scrollAmount == "undefined") {
      this._scrollAmount = pos;
    } else {
      // move the floating microphone at the same time with the screen
      var elem = document.getElementById("floatingMicrophone");
      elem.style.top = parseFloat(elem.style.top) + (pos - this._scrollAmount) + "px";
      this._scrollAmount = pos;
    }

    // pos/max will give you the distance between scroll bottom and and bottom of screen in percentage.
    if (
      pos == max ||
      Math.ceil(pos) == Math.ceil(max) ||
      Math.floor(pos) == Math.floor(max) ||
      Math.floor(pos) == Math.ceil(max) ||
      Math.ceil(pos) == Math.floor(max)
    ) {
      if (this._loadedFirstTime) {
        this.getCollections();
      }
    }
  }

  loadCollectionsUntilScrollbarAppears() {
    this.getCollections();
    let max = document.documentElement.scrollHeight;
    var interval = setInterval(() => {
      if (this._loadedFirstTime == true) {
        if (max < document.documentElement.scrollHeight || this._noMoreData == true) {
          clearInterval(interval);
        } else {
          this.getCollections();
        }
      }
    }, 1000);
  }

  toggleDeleteButton() {
    if (this._deleteAccent == this.bootstrapAccentPrimary) {
      this._deleteAccent = this.bootstrapAccentSecondary;
      this._modalDeleteConfirmation = "#deleteConfirmationModal";
    } else {
      this._deleteAccent = this.bootstrapAccentPrimary;
      this._modalDeleteConfirmation = "";
    }
  }

  accessOrDelete(collection) {
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  here put redirect on click
    if (this._deleteAccent == this.bootstrapAccentSecondary) {
      //console.log(collection);
      this._markedForDeletion = collection;
    } else {
      //console.log(collection);
      this.router.navigate([`/${collection.id}`]);
    }
  }

  accessRecommendedCollection(collection) {
    this.router.navigate([`/${collection.id}`]);
  }

  deleteCollection() {
    this.api
      .deleteData(
        this.deleteCollectionUrl.replace("/$collectionId", `/${this._markedForDeletion["id"]}`)
      )
      .pipe(take(1))
      .subscribe((data) => {});
    this._collectionData = this._collectionData.filter((item) => item != this._markedForDeletion);
  }

  addCollection() {
    //console.log(this._addNewCollectionForm);
    // let headers = new HttpHeaders();
    // headers.append("Content-Type", "application/json");
    this.api
      .postData(this.addCollectionUrl, {
        Name: `${this._addNewCollectionForm.value.collectionName}`,
        Type: this._addNewCollectionForm.value.collectionType,
        Keywords: `${this._addNewCollectionForm.value.keywords}`,
        UserId: `${this.userId}`
      })
      .pipe(take(1))
      .subscribe((data) => {
        //console.log(data);
        data["placeholder"] = [
          {
            data: "../../../assets/new-collection.jpg"
          }
        ];
        this._collectionData = [...this._collectionData, data];
      });
  }

  loadInputOptionsOrDefault() {
    this.configPath = "../../../assets/config.json";
    if (typeof this.configPath != "undefined") {
      this.api
        .getData(this.configPath)
        .pipe(
          catchError((err) => {
            this.loadDefault();
            if (typeof this.collectionUrl != "undefined") {
              this.loadCollectionsUntilScrollbarAppears();
              this.onWindowScroll();
            }
            return EMPTY;
          }),
          take(1),
          map((data) => {
            //console.log("here");
            return data;
          })
        )
        .subscribe((config) => {
          if (typeof config["userId"] != "undefined" && typeof this.userId == "undefined") {
            this.userId = config["userId"];
          }

          if (typeof config["gridSize"] != "undefined" && typeof this.gridSize == "undefined") {
            this.gridSize = config["gridSize"];
          }
          if (
            typeof config["gridSizeSuggestions"] != "undefined" &&
            typeof this.gridSizeSuggestions == "undefined"
          ) {
            this.gridSizeSuggestions = config["gridSizeSuggestions"];
          }
          if (
            typeof config["collectionUrl"] != "undefined" &&
            typeof this.collectionUrl == "undefined"
          ) {
            this.collectionUrl = config["collectionUrl"];
          }
          if (
            typeof config["suggestedCollectionUrl"] != "undefined" &&
            typeof this.suggestedCollectionUrl == "undefined"
          ) {
            this.suggestedCollectionUrl = config["suggestedCollectionUrl"];
          }
          if (
            typeof config["deleteCollectionUrl"] != "undefined" &&
            typeof this.deleteCollectionUrl == "undefined"
          ) {
            this.deleteCollectionUrl = config["deleteCollectionUrl"];
          }
          if (
            typeof config["addCollectionUrl"] != "undefined" &&
            typeof this.addCollectionUrl == "undefined"
          ) {
            this.addCollectionUrl = config["addCollectionUrl"];
          }
          if (typeof config["skip"] != "undefined" && typeof this.skip == "undefined") {
            this.skip = config["skip"];
          }
          if (typeof config["take"] != "undefined" && typeof this.take == "undefined") {
            this.take = config["take"];
          }
          if (
            typeof config["bootstrapAccentPrimary"] != "undefined" &&
            typeof this.bootstrapAccentPrimary == "undefined"
          ) {
            this.bootstrapAccentPrimary = config["bootstrapAccentPrimary"];
          }
          if (
            typeof config["bootstrapAccentSecondary"] != "undefined" &&
            typeof this.bootstrapAccentSecondary == "undefined"
          ) {
            this.bootstrapAccentSecondary = config["bootstrapAccentSecondary"];
          }
          if (typeof config["albumUrl"] != "undefined" && typeof this.albumUrl == "undefined") {
            this.albumUrl = config["albumUrl"];
          }
          if (
            typeof config["suggestedEntityUrl"] != "undefined" &&
            typeof this.suggestedEntityUrl == "undefined"
          ) {
            this.suggestedEntityUrl = config["suggestedEntityUrl"];
          }
          if (
            typeof config["deleteEntityUrl"] != "undefined" &&
            typeof this.deleteEntityUrl == "undefined"
          ) {
            this.deleteEntityUrl = config["deleteEntityUrl"];
          }
          if (
            typeof config["addEntitiesUrl"] != "undefined" &&
            typeof this.addEntitiesUrl == "undefined"
          ) {
            this.addEntitiesUrl = config["addEntitiesUrl"];
          }
          if (
            typeof config["getEntityUrl"] != "undefined" &&
            typeof this.getEntityUrl == "undefined"
          ) {
            this.getEntityUrl = config["getEntityUrl"];
          }
          if (typeof config["slideShow"] != "undefined" && typeof this.slideShow == "undefined") {
            this.slideShow = config["slideShow"];
          }
          if (
            typeof config["lockSlideShow"] != "undefined" &&
            typeof this.lockSlideShow == "undefined"
          ) {
            this.lockSlideShow = config["lockSlideShow"];
          }
          if (
            typeof config["slideShowTimeBeforeNext"] != "undefined" &&
            typeof this.slideShowTimeBeforeNext == "undefined"
          ) {
            this.slideShowTimeBeforeNext = config["slideShowTimeBeforeNext"];
          }
          this.loadDefault();
          if (typeof this.collectionUrl != "undefined") {
            this.loadCollectionsUntilScrollbarAppears();
            this.onWindowScroll();
          }
        });
    } else {
      this.loadDefault();
      if (typeof this.collectionUrl != "undefined") {
        this.loadCollectionsUntilScrollbarAppears();
        this.onWindowScroll();
      }
    }
  }

  loadDefault() {
    if (typeof this.userId == "undefined") {
      this.userId = "00000000-0000-0000-0000-000000000000";
    }
    if (typeof this.gridSize == "undefined") {
      this.gridSize = 10;
    }
    if (typeof this.gridSizeSuggestions == "undefined") {
      this.gridSizeSuggestions = 3;
    }
    if (typeof this.collectionUrl == "undefined") {
      this.collectionUrl = "";
    }
    if (typeof this.suggestedCollectionUrl == "undefined") {
      this.suggestedCollectionUrl = "";
    }
    if (typeof this.deleteCollectionUrl == "undefined") {
      this.deleteCollectionUrl = "";
    }
    if (typeof this.addCollectionUrl == "undefined") {
      this.addCollectionUrl = "";
    }
    if (typeof this.skip == "undefined") {
      this.skip = 0;
    }
    if (typeof this.take == "undefined") {
      this.take = 10;
    }
    if (typeof this.bootstrapAccentPrimary == "undefined") {
      this.bootstrapAccentPrimary = "danger";
    }
    if (typeof this.bootstrapAccentSecondary == "undefined") {
      this.bootstrapAccentSecondary = "dark";
    }
    if (typeof this.albumUrl == "undefined") {
      this.albumUrl = "";
    }
    if (typeof this.suggestedEntityUrl == "undefined") {
      this.suggestedEntityUrl = "";
    }
    if (typeof this.deleteEntityUrl == "undefined") {
      this.deleteEntityUrl = "";
    }
    if (typeof this.addEntitiesUrl == "undefined") {
      this.addEntitiesUrl = "";
    }
    if (typeof this.getEntityUrl == "undefined") {
      this.getEntityUrl = "";
    }
    if (typeof this.slideShow == "undefined") {
      this.slideShow = false;
    }
    if (typeof this.lockSlideShow == "undefined") {
      this.lockSlideShow = false;
    }
    if (typeof this.slideShowTimeBeforeNext == "undefined") {
      this.slideShowTimeBeforeNext = 5000;
    }

    var albumInputs = {
      gridSize: this.gridSize,
      gridSizeSuggestions: this.gridSizeSuggestions,
      skip: this.skip,
      take: this.take,
      bootstrapAccentPrimary: this.bootstrapAccentPrimary,
      bootstrapAccentSecondary: this.bootstrapAccentSecondary,
      albumUrl: this.albumUrl,
      suggestedEntityUrl: this.suggestedEntityUrl,
      deleteEntityUrl: this.deleteEntityUrl,
      addEntitiesUrl: this.addEntitiesUrl,
      getEntityUrl: this.getEntityUrl,
      lockSlideShow: this.lockSlideShow,
      slideShow: this.slideShow,
      slideShowTimeBeforeNext: this.slideShowTimeBeforeNext
    };
    sessionStorage.setItem("albumInputs", JSON.stringify(albumInputs));

    this._deleteAccent = this.bootstrapAccentPrimary;
  }
}
