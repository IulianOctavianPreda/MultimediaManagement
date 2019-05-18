import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: "root"
})
export class DataManagerService {
  role$: Observable<any>;
  private _role$: BehaviorSubject<any>;

  state$: Observable<any>;
  private _state$: BehaviorSubject<any>;

  token$: Observable<any>;
  private _token$: BehaviorSubject<any>;

  userId$: Observable<any>;
  private _userId$: BehaviorSubject<any>;

  constructor() {
    this.initialize();
  }
  initialize() {
    this._role$ = new BehaviorSubject(null);
    this.role$ = this._role$.asObservable();

    this._state$ = new BehaviorSubject(null);
    this.state$ = this._state$.asObservable();

    this._token$ = new BehaviorSubject(null);
    this.token$ = this._token$.asObservable();

    this._userId$ = new BehaviorSubject(null);
    this.userId$ = this._userId$.asObservable();
  }

  get role(): any {
    return this._role$.getValue();
  }
  setRole(nextRole: any): void {
    this._role$.next(nextRole);
  }

  get state(): any {
    return this._state$.getValue();
  }
  setState(next: any): void {
    this._state$.next(next);
  }

  get token(): any {
    return this._token$.getValue();
  }
  setToken(next: any): void {
    this._token$.next(next);
  }

  get userId(): any {
    return this._userId$.getValue();
  }
  setUserId(next: any): void {
    this._userId$.next(next);
  }
}
