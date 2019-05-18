import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: "app-lightbox",
  templateUrl: "./lightbox.component.html",
  styleUrls: ["./lightbox.component.scss"]
})
export class LightboxComponent implements OnInit, AfterViewInit, OnChanges {
  @Input()
  data;

  @Input()
  type;

  @Input()
  index;

  @Input()
  show;

  @Input()
  noMoreData;

  @Input()
  entityUrl;

  @Input()
  slideShow;

  @Input()
  lockSlideShow;

  @Input()
  slideShowTimeBeforeNext: number;

  @Output()
  noShow = new EventEmitter();

  @Output()
  loadMore = new EventEmitter();

  slideIndex;
  ready = 0;
  startSlideShow = false;

  constructor() {}

  ngOnInit() {
    if (typeof this.slideShow == "string") {
      this.slideShow = this.slideShow == "true";
    }
    if (typeof this.lockSlideShow == "string") {
      this.lockSlideShow = this.lockSlideShow == "true";
    }
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.ready = 1;
    }, 1000);
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.start();
  }

  start() {
    if (this.ready == 1) {
      if (this.show == 1) {
        this.slideIndex = this.index;
        this.openEntityModal();
        this.currentSlide(this.slideIndex + 1);
        //console.log(this.slideShow);
        if (this.slideShow == true && this.startSlideShow == false) {
          this.startSlideShow = true;
          setInterval(() => {
            this.plusSlides(1);
            //console.log("here");
          }, this.slideShowTimeBeforeNext);
        }
      }
    } else {
      setTimeout(this.start, 1000);
    }
  }

  openEntityModal() {
    document.getElementById(`${this.type}Modal`).style.display = "block";
  }
  closeEntityModal() {
    //console.log(typeof this.lockSlideShow);
    if (this.lockSlideShow == false) {
      document.getElementById(`${this.type}Modal`).style.display = "none";
      this.noShow.emit(0);
    }
  }
  plusSlides(n) {
    this.showSlides((this.slideIndex += n));
  }

  currentSlide(n) {
    this.showSlides((this.slideIndex = n));
  }

  showSlides(n) {
    var i;
    var slides = document.getElementsByClassName(`my${this.type}Slides`);
    // //console.log(slides);
    if (n > slides.length) {
      if (this.noMoreData) {
        this.slideIndex = 1;
      } else {
        this.loadMore.emit(true);
        this.slideIndex = this.slideIndex - 1;
      }
    }
    //console.log(slides);

    if (n < 1) {
      this.slideIndex = slides.length;
    }
    for (i = 0; i < slides.length; i++) {
      slides[i]["style"].display = "none";
    }

    slides[this.slideIndex - 1]["style"].display = "block";
  }
}
