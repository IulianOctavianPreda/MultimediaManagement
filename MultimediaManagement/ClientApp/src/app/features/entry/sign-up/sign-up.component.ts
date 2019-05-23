import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { catchError, switchMap, takeUntil } from 'rxjs/operators';
import { CryptoService } from 'src/app/shared/services/crypto-service.service';
import { DataManagerService } from 'src/app/shared/services/data-manager.service';

@Component({
  selector: "app-sign-up",
  templateUrl: "./sign-up.component.html",
  styleUrls: ["./sign-up.component.scss"]
})
export class SignUpComponent implements OnInit, OnDestroy {
  public formGroup: FormGroup;
  private destroy$: Subject<any> = new Subject();

  constructor(
    private formBuilder: FormBuilder,
    public http: HttpClient,
    public crypto: CryptoService,

    private toastr: ToastrService,
    public dataManager: DataManagerService,
    private router: Router
  ) {
    this.formGroup = this.formBuilder.group({
      username: null,
      password: null
    });
  }

  ngOnInit() {
    this.formGroup.controls["username"].setValidators([Validators.required]);
    this.formGroup.controls["password"].setValidators([
      Validators.required,
      Validators.minLength(8)
    ]);
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  signup() {
    if (this.formGroup.valid) {
      const signupData = this.formGroup.value;
      // signupData.password = this.crypto.sha256(signupData.password);

      this.http
        .post("http://localhost:49882/api/Users/signup", signupData)
        .pipe(
          takeUntil(this.destroy$),
          catchError((err) => {
            this.toastr.error("Invalid Credentials", "Username or password are wrong!");
            return of(null);
          }),
          switchMap((response) => {
            if (response != null) {
              this.dataManager.setToken(response.token);
              this.dataManager.setUserId(signupData.id);
              return this.http.post("http://localhost:49882/api/Security/getRole", {
                token: response.token
              });
            }
            return of(null);
          })
        )
        .subscribe((response) => {
          if (response != null) {
            this.toastr.success("Sign-up successful");
            this.dataManager.setRole(response.role);
            this.router.navigate(["/collections"]);
          }
        });
    }
  }
}
