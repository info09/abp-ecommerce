import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ProductAttributeInListDto, ProductAttributeService } from '@proxy/product-attributes';
import { ProductDto, ProductsService } from '@proxy/products';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { NotificationService } from '../shared/services/notification.service';
import { ConfirmationService } from 'primeng/api';
import { ProductAttributeValueDto } from '@proxy/products/attributes';
import { ProductAttributeType } from '@proxy/ecommerce/attributes';

@Component({
  selector: 'app-product-attribute',
  templateUrl: './product-attribute.component.html',
})
export class ProductAttributeComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  public form: FormGroup;
  attributes: any[] = [];
  fullAttributes: any[] = [];
  productAttributes: any[] = [];
  selectedEntity = {} as ProductDto;

  showDateTimeControl: boolean = false;
  showDecimalControl: boolean = false;
  showIntControl: boolean = false;
  showVarcharControl: boolean = false;
  showTextControl: boolean = false;

  constructor(
    private productAttributeService: ProductAttributeService,
    private productService: ProductsService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private notificationService: NotificationService,
    private confirmationService: ConfirmationService
  ) {}
  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.buildForm();
    this.initFormData();
  }

  initFormData() {
    var attributes = this.productAttributeService.getAllList();
    this.toggleBlockUI(true);
    forkJoin({ attributes })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: any) => {
          this.fullAttributes = response.attributes;
          var attributes = response.attributes as ProductAttributeInListDto[];
          attributes.forEach(element => {
            this.attributes.push({
              value: element.id,
              label: element.label,
            });
          });
          this.loadFormDetails(this.config.data?.id);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
      .getListProductAttributeAll(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: ProductAttributeValueDto[]) => {
          this.productAttributes = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private buildForm() {
    this.form = this.fb.group({
      productId: new FormControl(this.config.data.id),
      attributeId: new FormControl(null, Validators.required),
      dateTimeValue: new FormControl(null),
      decimalValue: new FormControl(null),
      intValue: new FormControl(null),
      varcharValue: new FormControl(null),
      textValue: new FormControl(null),
    });
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.productService
      .addProductAttribute(this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.loadFormDetails(this.config.data.id);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
  }

  selectAttribute(event: any) {
    var dataType = this.fullAttributes.filter(i => i.id == event.value)[0].productAttributeType;
    this.showDateTimeControl = false;
    this.showDecimalControl = false;
    this.showIntControl = false;
    this.showTextControl = false;
    this.showVarcharControl = false;
    if (dataType == ProductAttributeType.Date) {
      this.showDateTimeControl = true;
    } else if (dataType == ProductAttributeType.Decimal) {
      this.showDecimalControl = true;
    } else if (dataType == ProductAttributeType.Int) {
      this.showIntControl = true;
    } else if (dataType == ProductAttributeType.Text) {
      this.showTextControl = true;
    } else if (dataType == ProductAttributeType.Varchar) {
      this.showVarcharControl = true;
    }
  }

  getDataTypeName(value: number) {
    return ProductAttributeType[value];
  }

  getValueByType(attribute: ProductAttributeValueDto, value: number) {
    if (attribute.productAttributeType == ProductAttributeType.Date) {
      return attribute.dateTimeValue;
    } else if (attribute.productAttributeType == ProductAttributeType.Decimal) {
      return attribute.decimalValue;
    } else if (attribute.productAttributeType == ProductAttributeType.Int) {
      return attribute.intValue;
    } else if (attribute.productAttributeType == ProductAttributeType.Text) {
      return attribute.textValue;
    } else if (attribute.productAttributeType == ProductAttributeType.Varchar) {
      return attribute.varcharValue;
    }
  }

  removeItem(attribute: ProductAttributeValueDto) {
    var id = '';
    if (attribute.productAttributeType == ProductAttributeType.Date) {
      id = attribute.dateTimeId;
    } else if (attribute.productAttributeType == ProductAttributeType.Decimal) {
      id = attribute.decimalId;
    } else if (attribute.productAttributeType == ProductAttributeType.Int) {
      id = attribute.intId;
    } else if (attribute.productAttributeType == ProductAttributeType.Text) {
      id = attribute.textId;
    } else if (attribute.productAttributeType == ProductAttributeType.Varchar) {
      id = attribute.varcharId;
    }

    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa bản ghi này?',
      accept: () => {
        this.deleteItemsConfirmed(attribute.attributeId, id);
      },
    });
  }
  deleteItemsConfirmed(attributeId: string, id: string) {
    this.toggleBlockUI(true);
    this.productService
      .removeProductAttribute(attributeId, id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Xóa thành công');
          this.loadFormDetails(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = false;
      }, 1000);
    }
  }
}
