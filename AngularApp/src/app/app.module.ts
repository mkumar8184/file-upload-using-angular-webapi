import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { HomeComponent } from './common/component/home/home.component';
import { ErrorInterceptor } from './helpers/error.interceptor';
import { CommonAttachmentComponent } from './_partial/common-attachment/common-attachment.component';
import { Ng2ImgMaxModule } from "ng2-img-max";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    CommonAttachmentComponent

  ],
  imports: [
    BrowserModule,
    CommonModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    Ng2ImgMaxModule
   
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
   
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
