import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: "root"
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree | any {
    return this.authService.isAuthenticated().pipe(
      map((e) => {
        return true;
      }),
      catchError((err) => {
        this.router.navigate([""]);
        return of(false);
      })
    );
  }
}

// if (this.authService.isAuthenticated()) {
//   return true;
// }

// // navigate to login page
// this.router.navigate(['']);
// // you can save redirect url so after authing we can move them back to the page they requested
// return false;
