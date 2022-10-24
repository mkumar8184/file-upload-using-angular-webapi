import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

isSingle=false;
FileTypeId=1;
attachementIds:any;

  constructor() { }

  ngOnInit(): void {

  }
  public setAttachmentId(attachment: any): void {

      AttachmentIds: attachment.id.toString()

  }

  


}
