import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';

@NgModule({
  declarations: [ProductComponent],
  imports: [
    SharedModule,
    TableModule,
    PanelModule,
    PaginatorModule,
    BlockUIModule,
    ProductRoutingModule,
  ],
})
export class ProductModule {}
