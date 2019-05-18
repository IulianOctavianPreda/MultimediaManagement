import { Injectable } from '@angular/core';
import * as Hammer from 'hammerjs';

@Injectable({
  providedIn: "root"
})
export class FloatingMicrophoneService {
  constructor() {}

  // if it doesn't work right, remove touch events and uncomment prevent default
  makeFloatingMicrophone(speechComponent) {
    var elem = document.getElementById("floatingMicrophone");
    var moveFlag = false;
    var speech = speechComponent;
    var pos1 = 0,
      pos2 = 0,
      pos3 = 0,
      pos4 = 0;
    elem.style.top = window.innerHeight - 75 + "px";
    elem.style.left = window.innerWidth - 75 + "px";
    if (elem) {
      elem.onmousedown = dragMouseDown;
    }
    if (
      /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)
    ) {
      this.touchInputsListeners();
    }

    function toggleMic() {
      if (elem.classList.contains("notRecording")) {
        elem.classList.remove("notRecording");
        elem.classList.add("recording");
        speech.start();
      } else {
        elem.classList.remove("recording");
        elem.classList.add("notRecording");
        speech.stop();
      }
    }

    function dragMouseDown(e) {
      moveFlag = false;
      e = e || window.event;
      e.preventDefault();
      // get the mouse cursor position at startup:
      pos3 = e.clientX;
      pos4 = e.clientY;
      document.onmouseup = closeDragElement;

      // call a function whenever the cursor moves:
      document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
      moveFlag = true;
      e = e || window.event;
      e.preventDefault();
      // calculate the new cursor position:
      pos1 = pos3 - e.clientX;
      pos2 = pos4 - e.clientY;
      pos3 = e.clientX;
      pos4 = e.clientY;
      // set the element's new position:
      elem.style.top = elem.offsetTop - pos2 + "px";
      elem.style.left = elem.offsetLeft - pos1 + "px";
    }

    function closeDragElement() {
      /* stop moving when mouse button is released:*/
      document.onmouseup = null;
      document.onmousemove = null;

      if (moveFlag == false) {
        toggleMic();
      }
    }
  }

  touchInputsListeners() {
    var elem = document.getElementById("floatingMicrophone");

    var mc = new Hammer(elem);
    mc.add(new Hammer.Pan({ direction: Hammer.DIRECTION_ALL, threshold: 0 }));
    mc.on("pan", handleDrag);

    // drag event
    var lastPosX = 0;
    var lastPosY = 0;
    var isDragging = false;
    function handleDrag(ev) {
      if (!isDragging) {
        isDragging = true;
        lastPosX = elem.offsetLeft;
        lastPosY = elem.offsetTop;
      }
      var posX = ev.deltaX + lastPosX;
      var posY = ev.deltaY + lastPosY;
      elem.style.left = posX + "px";
      elem.style.top = posY + "px";
      if (ev.isFinal) {
        isDragging = false;
      }
    }
  }
}
