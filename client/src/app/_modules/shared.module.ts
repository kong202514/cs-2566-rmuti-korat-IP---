import { NgModule } from '@angular/core'
import { CommonModule } from '@angular/common'
import { BsDropdownModule } from 'ngx-bootstrap/dropdown'
import { ButtonsModule } from 'ngx-bootstrap/buttons'
import { ToastrModule } from 'ngx-toastr'
import { TabsModule } from 'ngx-bootstrap/tabs'
import { NgxSpinnerModule } from 'ngx-spinner'
import { FileUploadModule } from 'ng2-file-upload'
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { TimeagoModule } from "ngx-timeago"

import { ModalModule } from 'ngx-bootstrap/modal'
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FileUploadModule,
    TimeagoModule.forRoot(),
    PaginationModule.forRoot(),
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot(),
    TabsModule.forRoot(),
    BsDatepickerModule.forRoot(),
    ButtonsModule.forRoot(),
    NgxSpinnerModule.forRoot({ type: 'square-spin' }),
    ModalModule.forRoot(),




  ],
  exports: [
    TimeagoModule,
    PaginationModule,
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    NgxSpinnerModule,
    FileUploadModule,
    BsDatepickerModule,
    ButtonsModule,
    ModalModule,
  ]
})
export class SharedModule { }
