import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: "root"
})
export class CryptoService {
  constructor() {
    console.log(this.sha256("Message"));
  }

  sha256(message) {
    return CryptoJS.SHA256(message);
  }
  // md5Hex(message) {
  //   return toHex(CryptoJS.MD5, message);
  // }

  // sha1Hex(message) {
  //   return toHex(CryptoJS.SHA1, message);
  // }

  // sha256Hex(message) {
  //   return toHex(CryptoJS.SHA256, message);
  // }

  // sha224Hex(message) {
  //   return toHex(CryptoJS.SHA224, message);
  // }

  // sha512Hex(message) {
  //   return toHex(CryptoJS.SHA512, message);
  // }

  // sha384Hex(message) {
  //   return toHex(CryptoJS.SHA384, message);
  // }

  // sha3Hex(message) {
  //   return toHex(CryptoJS.SHA3, message);
  // }

  // toHex(hasher, message) {
  //   return hasher(message).toString(CryptoJS.enc.Hex);
  // }
}
