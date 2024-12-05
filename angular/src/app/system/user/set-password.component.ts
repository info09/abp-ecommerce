import { AuthService } from '@abp/ng.core';
import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { RoleDto } from '@proxy/system/roles';
import { UserService } from '@proxy/system/users';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';

@Component({
  templateUrl: './set-password.component.html',
})
export class SetPasswordComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  // Default
  public blockedPanelDetail: boolean = false;
  public form: FormGroup;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public closeBtnName: string;
  selectedEntity = {} as RoleDto;
  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private userService: UserService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {}

  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    passsword: [
      { type: 'required', message: 'Bạn phải nhập mật khẩu' },
      {
        type: 'pattern',
        message: 'Mật khẩu ít nhất 8 ký tự, ít nhất 1 số, 1 ký tự đặc biệt, và một chữ hoa',
      },
    ],
    confirmPassword: [{ type: 'required', message: 'Xác nhận mật khẩu không đúng' }],
  };

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.buildForm();
    this.saveBtnName = 'Cập nhật';
    this.closeBtnName = 'Hủy';
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.userService
      .setPassword(this.config.data?.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.ref.close(true);
        },
      });
  }

  buildForm() {
    this.form = this.fb.group(
      {
        newPassword: new FormControl(
          null,
          Validators.compose([
            Validators.required,
            Validators.pattern(
              '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$'
            ),
          ])
        ),
        confirmNewPassword: new FormControl(null, Validators.required),
      },
      { validators: passwordMatchingValidator }
    );
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
}

export const passwordMatchingValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {
  if (!control) {
    return null; // Không làm gì nếu không có control
  }

  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmNewPassword');

  if (!password || !confirmPassword) {
    return null; // Nếu không tìm thấy các control, bỏ qua kiểm tra
  }

  if (confirmPassword.errors && !confirmPassword.errors['notmatched']) {
    return null; // Không ghi đè các lỗi khác đã tồn tại
  }

  // Kiểm tra giá trị của hai control
  if (password.value !== confirmPassword.value) {
    confirmPassword.setErrors({ notmatched: true });
  } else {
    confirmPassword.setErrors(null); // Xóa lỗi nếu hai giá trị khớp nhau
  }

  return null; // ValidatorForm trả về null nếu hợp lệ
};
