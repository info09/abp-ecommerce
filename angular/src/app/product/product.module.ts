import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { ProductDetailComponent } from './product-detail.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { EditorModule } from 'primeng/editor';
import { TeduSharedModule } from '../shared/modules/tedu-shared.module';
import { BadgeModule } from 'primeng/badge';
import { ImageModule } from 'primeng/image';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ProductAttributeComponent } from './product-attribute.component';

@NgModule({
  declarations: [ProductComponent, ProductDetailComponent, ProductAttributeComponent],
  imports: [
    SharedModule,
    TableModule,
    PanelModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    ProductRoutingModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    InputNumberModule,
    CheckboxModule,
    InputTextareaModule,
    EditorModule,
    TeduSharedModule,
    BadgeModule,
    ImageModule,
    ConfirmDialogModule,
  ],
  entryComponents: [ProductDetailComponent, ProductAttributeComponent],
})
export class ProductModule {}
