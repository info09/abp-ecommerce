import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { ProductDto, ProductsService } from '@proxy/products';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { UtilityService } from '../shared/services/utility.service';
import { ManufacturerInListDto, ManufacturersService } from '@proxy/manufacturers';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { productTypeOptions } from '@proxy/ecommerce/products';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  public form: FormGroup;
  btnDisabled = false;

  productCategories: any[] = [];
  manufacturers: any[] = [];
  productTypes: any[] = [];

  selectedEntity = {} as ProductDto;

  constructor(
    private productService: ProductsService,
    private productCategoryService: ProductCategoriesService,
    private manufacturerService: ManufacturersService,
    private fb: FormBuilder,
    private config: DynamicDialogConfig,
    private ref: DynamicDialogRef,
    private utilityService: UtilityService
  ) {}

  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    name: [
      { type: 'required', message: 'Bạn phải nhập tên' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    sku: [{ type: 'required', message: 'Bạn phải mã SKU sản phẩm' }],
    manufacturerId: [{ type: 'required', message: 'Bạn phải chọn nhà cung cấp' }],
    categoryId: [{ type: 'required', message: 'Bạn phải chọn danh mục' }],
    productType: [{ type: 'required', message: 'Bạn phải chọn loại sản phẩm' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
    sellPrice: [{ type: 'required', message: 'Bạn phải nhập giá bán' }],
  };

  ngOnDestroy(): void {}
  ngOnInit(): void {
    this.buildForm();
    this.loadProductType();
    this.initFormData();
  }

  initFormData() {
    //Load data to form
    var productCategories = this.productCategoryService.getListAll();
    var manufacturers = this.manufacturerService.getListAll();

    this.toggleBlockUI(true);
    forkJoin({ productCategories, manufacturers })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: any) => {
          var productCategories = res.productCategories as ProductCategoryInListDto[];
          var manufacturers = res.manufacturers as ManufacturerInListDto[];

          productCategories.forEach(element => {
            this.productCategories.push({
              value: element.id,
              label: element.name,
            });
          });

          manufacturers.forEach(element => {
            this.manufacturers.push({
              value: element.id,
              label: element.name,
            });
          });

          if (this.utilityService.isEmpty(this.config.data?.id) == true) {
            this.toggleBlockUI(true);
          } else {
            this.loadFormDetails(this.config.data?.id);
          }
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  generateSlug() {
    this.form.controls['slug'].setValue(
      this.utilityService.MakeSeoTitle(this.form.get('name').value)
    );
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: ProductDto) => {
          this.selectedEntity = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: err => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChange() {
    this.toggleBlockUI(true);
    if (this.utilityService.isEmpty(this.config.data?.id) == true) {
      this.productService
        .create(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.productService
        .update(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }
  }
  loadProductType() {
    productTypeOptions.forEach(element => {
      this.productTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || false),
      isActive: new FormControl(this.selectedEntity.isActive || false),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
