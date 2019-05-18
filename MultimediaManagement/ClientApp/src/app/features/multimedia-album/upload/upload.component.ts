import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import * as $ from 'jquery';
import * as pdfjsLib from 'pdfjs-dist';
import { take } from 'rxjs/operators';
import { ApiService } from 'src/app/shared/api/api.service';

@Component({
  selector: "app-upload",
  templateUrl: "./upload.component.html",
  styleUrls: ["./upload.component.scss"]
})
export class UploadComponent implements OnInit, OnChanges, AfterViewInit {
  @Input()
  upload;

  @Input()
  cancel;

  @Input()
  collectionId;

  @Input()
  addEntitiesUrl;

  @Input()
  bootstrapAccentSecondary;

  @Output()
  newData = new EventEmitter();

  @Output()
  discarded = new EventEmitter();

  imageFormats = ["jpeg", "jpg", "png", "webp", "gif"];
  videoFormats = ["webm", "ogv", "mp4"];
  audioFormats = ["mp3", "wave", "ogg", "oga"];
  bypassCors = "http://cors-anywhere.herokuapp.com/";
  arrayOfEntityAndPlaceholder = [];
  urlInput = "";
  constructor(public api: ApiService) {}

  ngOnInit() {
    this.makeUploadArea();
    // setInterval(() => {
    //   //console.log(this.arrayOfEntityAndPlaceholder);
    // }, 1000);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.upload == true) {
      this.uploadFiles();
    }
    if (this.cancel == true) {
      this.uploadFiles();
    }
  }

  ngAfterViewInit(): void {}

  discardFile(entity) {
    this.arrayOfEntityAndPlaceholder = this.arrayOfEntityAndPlaceholder.filter(
      (item) => item != entity
    );
    //console.log(this.arrayOfEntityAndPlaceholder);
  }

  discardChanges() {
    this.arrayOfEntityAndPlaceholder = [];
    this.discarded.emit(0);
  }

  uploadFiles() {
    //console.log(this.addEntitiesUrl);
    //console.log(this.arrayOfEntityAndPlaceholder);
    var sendData = [];
    var totalStorage = 0;
    this.arrayOfEntityAndPlaceholder.forEach((entity) => {
      //console.log(entity.fileSize);
      if (entity.fileSize <= 21) {
        if (totalStorage <= 21 && totalStorage + entity.fileSize <= 21) {
          sendData.push(entity);
          totalStorage += entity.fileSize;
        } else {
          this.postData(sendData);
          totalStorage = 0;
          sendData = [];

          sendData.push(entity);
          totalStorage += entity.fileSize;
        }
      }
    });
    if (sendData.length > 0) {
      this.postData(sendData);
    }
    this.arrayOfEntityAndPlaceholder = [];
  }

  postData(sendData) {
    this.api
      .postData(this.addEntitiesUrl, sendData)
      .pipe(take(1))
      .subscribe((data) => {
        for (var i = 0; i < sendData.length; i++) {
          sendData[i]["entityFile"] = null;
          sendData[i]["id"] = data[i]["id"];
        }
        this.newData.emit(sendData);
      });
  }

  makeUploadArea() {
    $("#uploadModal").on({
      "dragover dragenter": (e) => {
        e.preventDefault();
        e.stopPropagation();
      },
      drop: (e, ui) => {
        var dataTransfer = e.originalEvent["dataTransfer"];
        //console.log(dataTransfer);
        if (dataTransfer && dataTransfer.files.length) {
          //console.log(dataTransfer);
          e.preventDefault();
          e.stopPropagation();
          this.readUploadedFilesAndGetPlaceholders(dataTransfer.files, this.collectionId);
        }
      }
    });
  }

  uploadFilesFromButton(event) {
    this.readUploadedFilesAndGetPlaceholders(event.target.files, this.collectionId);
  }

  readUploadedFilesAndGetPlaceholders(e, collectionId) {
    const width = 200;
    const height = 200;
    //console.log(e);
    Object.keys(e).forEach((file) => {
      //console.log(e[file]);
      //console.log(e[file].name);

      const fileName = e[file].name;
      const fileSize = e[file].size / 1024000;

      const reader = new FileReader();
      reader.readAsDataURL(e[file]);
      (reader.onload = (event) => {
        var readerMetaData = event.target["result"].split(";")[0];
        if (readerMetaData.includes("image") && readerMetaData.includes("svg")) {
          this.imageData(
            event.target["result"],
            width,
            height,
            "svg",
            fileName,
            0,
            collectionId,
            fileSize
          );
        }
        if (readerMetaData.includes("image") && !readerMetaData.includes("svg")) {
          this.imageData(
            event.target["result"],
            width,
            height,
            "image",
            fileName,
            0,
            collectionId,
            fileSize
          );
        }
        if (readerMetaData.includes("audio")) {
          this.audioData(
            event.target["result"],
            width,
            height,
            "audio",
            fileName,
            0,
            collectionId,
            fileSize
          );
        }
        if (readerMetaData.includes("video")) {
          this.videoData(
            event.target["result"],
            width,
            height,
            "video",
            fileName,
            0,
            collectionId,
            fileSize
          );
        }
        if (readerMetaData.includes("pdf")) {
          this.pdfData(
            event.target["result"],
            width,
            height,
            "pdf",
            fileName,
            0,
            collectionId,
            fileSize
          );
        }
        //console.log(this.arrayOfEntityAndPlaceholder);
      }),
        (reader.onerror = (error) => console.log(error));
    });
  }

  urlInputClass = "";
  scrapUrl() {
    this.getPlaceholderFromUrl(this.collectionId);
  }

  getPlaceholderFromUrl(collectionId) {
    const width = 200;
    const height = 200;

    var url = this.urlInput.split("?")[0];
    var extensionArr = url.split(".");
    var extension = extensionArr[extensionArr.length - 1];
    var fileSize = 0.05;
    var fileName = "File From Url";
    if (typeof url != "undefined") {
      if (
        this.imageFormats.includes(extension) ||
        this.audioFormats.includes(extension) ||
        this.videoFormats.includes(extension) ||
        extension == "pdf" ||
        extension == "svg" ||
        url.includes("youtube")
      ) {
        if (extension == "svg") {
          this.imageData(url, width, height, "svg", fileName, 1, collectionId, fileSize);
        }
        if (this.imageFormats.includes(extension)) {
          this.imageData(url, width, height, "image", fileName, 1, collectionId, fileSize);
        }
        if (this.audioFormats.includes(extension)) {
          this.audioData(url, width, height, "audio", fileName, 1, collectionId, fileSize);
        }
        if (this.videoFormats.includes(extension)) {
          this.videoData(url, width, height, "video", fileName, 1, collectionId, fileSize);
        }
        if (extension == "pdf") {
          this.pdfData(url, width, height, "pdf", fileName, 1, collectionId, fileSize);
        }
        if (url.includes("youtube")) {
          this.youtubeData(
            this.urlInput,
            width,
            height,
            "youtube",
            fileName,
            1,
            collectionId,
            fileSize
          );
        }
        this.urlInputClass = "";
      } else {
        this.urlInputClass = "text-danger";
      }
    } else {
      this.urlInputClass = "text-danger";
    }
    this.urlInput = "";
  }

  generatePostObject(data, canvas, extension, fileName, isUrl, collectionId, fileSize) {
    return {
      name: fileName.split(".")[0],
      data: canvas.toDataURL("image/jpeg", 1),
      collectionId: collectionId,
      extension: extension,
      keywords: "",
      fileSize: fileSize,
      entityFile: [
        {
          name: fileName,
          extension: extension,
          isUrl: isUrl,
          data: data
        }
      ]
    };
  }

  imageData(data, width, height, extension, fileName, isUrl, collectionId, fileSize) {
    const img = new Image();
    if (isUrl == 1) {
      img.crossOrigin = "Anonymous";
      img.src = this.bypassCors + data;
    } else {
      img.src = data;
    }
    img.onload = () => {
      const canvas = document.createElement("canvas");
      canvas.width = width;
      canvas.height = height;
      canvas.getContext("2d").drawImage(img, 0, 0, width, height);

      this.arrayOfEntityAndPlaceholder.push(
        this.generatePostObject(data, canvas, extension, fileName, isUrl, collectionId, fileSize)
      );
      //console.log(this.arrayOfEntityAndPlaceholder);
    };
  }

  audioData(data, width, height, extension, fileName, isUrl, collectionId, fileSize) {
    const img = new Image();
    img.src = "../../../assets/music-icon.jpg";
    img.onload = () => {
      const canvas = document.createElement("canvas");
      canvas.width = width;
      canvas.height = height;
      canvas.getContext("2d").drawImage(img, 0, 0, width, height);
      this.arrayOfEntityAndPlaceholder.push(
        this.generatePostObject(data, canvas, extension, fileName, isUrl, collectionId, fileSize)
      );
    };
  }
  pdfData(data, width, height, extension, fileName, isUrl, collectionId, fileSize) {
    var dataBypass;
    if (isUrl == 1) {
      dataBypass = this.bypassCors + data;
    } else {
      dataBypass = data;
    }
    pdfjsLib.getDocument(dataBypass).then((pdf) => {
      pdf.getPage(1).then((page) => {
        var canvas = document.createElement("canvas");
        var viewport = page.getViewport(1.0);
        var context = canvas.getContext("2d");

        canvas.height = height;
        canvas.width = width;

        page
          .render({
            canvasContext: context,
            viewport: viewport
          })
          .then(() => {
            this.arrayOfEntityAndPlaceholder.push(
              this.generatePostObject(
                data,
                canvas,
                extension,
                fileName,
                isUrl,
                collectionId,
                fileSize
              )
            );
          });
      });
    });
  }

  videoData(data, width, height, extension, fileName, isUrl, collectionId, fileSize) {
    const video = document.createElement("video");
    if (isUrl == 1) {
      video.crossOrigin = "Anonymous";
      video.src = this.bypassCors + data;
    } else {
      video.src = data;
    }
    video.onloadeddata = (e) => {
      const canvas = document.createElement("canvas");
      canvas.width = width;
      canvas.height = height;
      canvas.getContext("2d").drawImage(video, 0, 0, width, height);
      this.arrayOfEntityAndPlaceholder.push(
        this.generatePostObject(data, canvas, extension, fileName, isUrl, collectionId, fileSize)
      );
    };
  }

  youtubeData(data, width, height, extension, fileName, isUrl, collectionId, fileSize) {
    var url = "https://img.youtube.com/vi/" + data.split("/watch?v=")[1] + "/0.jpg";
    const img = new Image();
    img.crossOrigin = "Anonymous";
    img.src = this.bypassCors + url;
    img.onload = () => {
      const canvas = document.createElement("canvas");
      canvas.width = width;
      canvas.height = height;
      canvas.getContext("2d").drawImage(img, 0, 0, width, height);
      this.arrayOfEntityAndPlaceholder.push(
        this.generatePostObject(data, canvas, extension, fileName, isUrl, collectionId, fileSize)
      );
    };
  }
}
