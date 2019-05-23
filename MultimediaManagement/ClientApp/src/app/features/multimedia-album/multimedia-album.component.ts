import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpeechService } from 'ngx-speech';
import { EMPTY, Subject } from 'rxjs';
import { catchError, take, takeUntil } from 'rxjs/operators';
import { ApiService } from 'src/app/shared/api/api.service';
import { SnakeService } from 'src/app/shared/easterEgg/snake.service';
import { FloatingMicrophoneService } from 'src/app/shared/services/floating-microphone.service';

@Component({
  selector: "app-multimedia-album",
  templateUrl: "./multimedia-album.component.html",
  styleUrls: ["./multimedia-album.component.scss"]
})
export class MultimediaAlbumComponent implements OnInit, OnDestroy {
  collectionId;
  gridSize;
  gridSizeSuggestions;
  skip;
  take;
  albumUrl;
  suggestedEntityUrl;
  deleteEntityUrl;
  addEntitiesUrl;
  getEntityUrl;
  bootstrapAccentPrimary;
  bootstrapAccentSecondary;
  slideShow;
  lockSlideShow;
  slideShowTimeBeforeNext;

  _albumData = [];
  _searchText;
  _mostUsedKeywords;
  _loadedFirstTime = false;
  _noMoreData = false;
  _deleteAccent;
  _markedForDeletion;
  _modalDeleteConfirmation = "";
  _scrollAmount;
  _suggestedEntities;
  _loadedEntity;
  _showEntityModal;
  _slideIndex = 1;
  _showSuggestedEntityModal = 0;
  _suggestedSlideIndex = 1;
  private _destroyed = new Subject<void>();
  constructor(
    public api: ApiService,
    public speech: SpeechService,
    public snake: SnakeService,
    public router: Router,
    private route: ActivatedRoute,
    private floatingMicrophone: FloatingMicrophoneService
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.collectionId = params["id"];
    });
    this.loadInputOptions();
    this.speechActions();
    this.floatingMicrophone.makeFloatingMicrophone(this.speech);
    this.lockSlideShow;
    if (this.slideShow) {
      this._showEntityModal = 1;
    } else {
      this._showEntityModal = 0;
    }
  }

  ngOnDestroy(): void {
    this._destroyed.next();
    this._destroyed.complete();
  }

  //to modify actions
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
        //console.log(msg);
        if (msg.message == "delete") {
          document.getElementById("deleteEntityButton").click();
        }
        if (msg.message == "add") {
          document.getElementById("addEntityButton").click();
        }
        if (
          msg.message == "suggestions" ||
          msg.message == "tip" ||
          msg.message == "recommendations"
        ) {
          document.getElementById("suggestionsEntityButton").click();
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

  //delete and add is missing

  getAlbum() {
    this.api
      .getData(
        this.albumUrl
          .replace("/$collectionId", `/${this.collectionId}`)
          .replace("/$take", `/${this.take}`)
          .replace("/$skip", `/${this.skip}`)
      )
      .pipe(take(1))
      .subscribe((data) => {
        if (typeof this._albumData == "undefined") {
          this._albumData = [];
        }
        if (data["placeholder"].length != 0) {
          //console.log(this._albumData);
          this._albumData = [...this._albumData, ...data["placeholder"]];
          this.skip += this.take;
          var str = "";
          this._albumData.forEach((placeholder) => {
            str += "," + placeholder["keywords"];
          });
          this._mostUsedKeywords = this.mostUsedKeywords(str);
          //console.log(typeof this.suggestedEntityUrl);
          if (typeof this.suggestedEntityUrl != "undefined") {
            //console.log(typeof this.suggestedEntityUrl);

            this.getSuggestedEntities();
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
    wordsList = wordsList.replace(/,null/g, "");
    //console.log(wordsList);
    return wordsList;
  }

  getSuggestedEntities() {
    //for suggestions we skip 0 since we want only the best suggestions
    //we use collection id to skip those from the search results since we already have them
    this.api
      .getData(
        this.suggestedEntityUrl
          .replace("/$collectionId", `/${this.collectionId}`)
          .replace("/$take", `/${this.take}`)
          .replace("/$skip", `/0`) + `/${this._mostUsedKeywords}`
      )
      .pipe(take(1))
      .subscribe((data: Array<{}>) => {
        this._suggestedEntities = data;
        //console.log(data);
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
        this.getAlbum();
      }
    }
  }

  accessOrDelete(placeholder, i) {
    if (this._deleteAccent == this.bootstrapAccentSecondary) {
      this._markedForDeletion = placeholder;
      //console.log(this._markedForDeletion);
      //console.log(placeholder);
    } else {
      this._showEntityModal = 1;
      this._slideIndex = i;
    }
  }
  noEntityModal(event) {
    //console.log(event);
    this._showEntityModal = event;
  }

  accessSuggested(i) {
    this._showSuggestedEntityModal = 1;
    this._suggestedSlideIndex = i;
  }

  noSuggestedEntityModal(event) {
    //console.log(event);
    this._showSuggestedEntityModal = event;
  }

  deleteEntity() {
    this.api
      .deleteData(this.deleteEntityUrl.replace("/$entityId", `/${this._markedForDeletion["id"]}`))
      .pipe(take(1))
      .subscribe((data) => {});
    this._albumData = this._albumData.filter((item) => item != this._markedForDeletion);
  }

  loadEntitiesUntilScrollbarAppears() {
    this.getAlbum();
    let max = document.documentElement.scrollHeight;
    var interval = setInterval(() => {
      if (this._loadedFirstTime == true) {
        if (max < document.documentElement.scrollHeight || this._noMoreData == true) {
          clearInterval(interval);
        } else {
          this.getAlbum();
        }
      }
    }, 1000);
  }

  loadMore(event) {
    this.getAlbum();
  }

  addNewData(event) {
    //console.log(this._albumData);

    this._albumData = [...this._albumData, ...event];
    //console.log(this._albumData);

    this._upload = false;
  }

  _upload = false;
  upload() {
    this._upload = true;
  }

  _cancel = false;
  cancel() {
    this._cancel = true;
  }

  discardedData(event) {
    this._cancel = event;
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

  loadInputOptions() {
    var albumInputOptions = JSON.parse(sessionStorage.getItem("albumInputs"));
    this.gridSize = albumInputOptions.gridSize;
    this.gridSizeSuggestions = albumInputOptions.gridSizeSuggestions;
    this.skip = albumInputOptions.skip;
    this.take = albumInputOptions.take;
    this.albumUrl = albumInputOptions.albumUrl;
    this.suggestedEntityUrl = albumInputOptions.suggestedEntityUrl;
    this.deleteEntityUrl = albumInputOptions.deleteEntityUrl;
    this.addEntitiesUrl = albumInputOptions.addEntitiesUrl;
    this.getEntityUrl = albumInputOptions.getEntityUrl;
    this.lockSlideShow = albumInputOptions.lockSlideShow;
    this.slideShow = albumInputOptions.slideShow;
    this.slideShowTimeBeforeNext = albumInputOptions.slideShowTimeBeforeNext;
    this.bootstrapAccentPrimary = albumInputOptions.bootstrapAccentPrimary;
    this.bootstrapAccentSecondary = albumInputOptions.bootstrapAccentSecondary;
    this._deleteAccent = this.bootstrapAccentPrimary;

    if (typeof this.albumUrl != "undefined") {
      this.loadEntitiesUntilScrollbarAppears();
      this.onWindowScroll();
    }
  }
}
