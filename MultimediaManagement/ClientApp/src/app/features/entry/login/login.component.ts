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
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit, OnDestroy {
  public formGroup: FormGroup;
  private destroy$: Subject<any> = new Subject();

  constructor(
    private formBuilder: FormBuilder,
    public crypto: CryptoService,
    public http: HttpClient,
    private toastr: ToastrService,
    public dataManager: DataManagerService,
    private router: Router
  ) {
    this.formGroup = this.formBuilder.group({
      username: null,
      password: null,
      role: null
    });
    this.formGroup.controls["role"].valueChanges.subscribe((value) => {
      console.log(value);
      if (value == "guest") {
        this.formGroup.controls["username"].clearValidators();
        this.formGroup.controls["password"].clearValidators();
        this.formGroup.patchValue({ username: "guest", password: null });
      } else {
        this.formGroup.controls["username"].setValidators([Validators.required]);
        this.formGroup.controls["password"].setValidators([
          Validators.required,
          Validators.minLength(8)
        ]);
        this.formGroup.patchValue({ username: null, password: null });
      }
    });
  }

  ngOnInit() {
    this.formGroup.controls["role"].setValidators([Validators.required]);
    this.formGroup.patchValue({ role: "user" });
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  login() {
    if (this.formGroup.valid) {
      const loginData = this.formGroup.value;
      // loginData.password = this.crypto.sha256(loginData.password);
      console.log(loginData);
      this.http
        .post("http://localhost:49882/api/Users/login", loginData)
        .pipe(
          takeUntil(this.destroy$),
          catchError((err) => {
            this.toastr.error("Invalid Credentials", "Username or password are wrong!");
            return of(null);
          }),
          switchMap((response) => {
            if (response != null) {
              this.dataManager.setToken(response.token);
              this.dataManager.setUserId(response.id);
              return this.http.post("http://localhost:49882/api/Security/getRole", {
                token: response.token
              });
            }
            return of(null);
          })
        )
        .subscribe((response) => {
          if (response != null) {
            this.dataManager.setRole(response.role);
            console.log(this.dataManager.role, this.dataManager.token);
            this.router.navigate(["/collections"]);
          }
        });
      //loginData.password = this.crypto.sha256(loginData.password);
    }
  }
}
