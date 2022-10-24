import {
  Component,
  OnInit,
  ViewEncapsulation,
  Input,
  EventEmitter,
  Output

} from "@angular/core";

import { AttachmentService } from "./attachment.service";
import { fileModel } from "./fileModel";
import { saveAs } from 'file-saver';
import { Ng2ImgMaxService } from "ng2-img-max";

@Component({
  selector: "app-common-attachment",
  templateUrl: "./common-attachment.component.html",
  styleUrls: ["./common-attachment.component.scss"],
  encapsulation: ViewEncapsulation.None
})
export class CommonAttachmentComponent implements OnInit {
  @Input() FileName: string = "";
  @Input() IsMandatory: boolean = true;
  @Input() hideTitle: boolean = false;
  @Input() fontSize: string = "f-14";
  @Input() isSingle = true;
  @Input() FileTypeId: number = 1;
  @Input() hideDelete: boolean = false;
  @Input() allowedExtentions: Array<string>;
  @Input() resizeFile: boolean = false;
  @Input() resizeInWidth: number = 0;
  @Input() resizeInHeight: number = 0;
  filesList: Array<fileModel> = [];
  errorMessage!: string;
  isFileLoaded = false;
  fileName: string = "";
  loading = false;
  loadingDelete = false;
  loadingDownload = false;

  @Output() attachment: EventEmitter<any> = new EventEmitter<any>();
  data: any;

  constructor(
    private attachmentService: AttachmentService,
    private ng2ImgMax: Ng2ImgMaxService,
  ) { }

  ngOnInit() {
   
  }


  onUpload(element: any) {
    this.data = element.files[0];
    this.fileName = this.data.name;
    this.errorMessage = "";
    this.loading = true;
    if (!this.validateFiles(element)) // validate 
      return;

    if (this.resizeFile) {
      this.resizeFiles(this.data);
    }
    else {
      this.uploadFiles(this.data);
    }
    this.loading = false;

  }
  resizeFiles(file: any) {
    if (this.resizeFile == true) {
      this.ng2ImgMax.resizeImage(file, this.resizeInWidth, this.resizeInHeight)
        .subscribe({
          error: (e: any) => {
            this.loading = false;
            console.log(e);
          },    // errorHandler 
          next: (v: any) => {
            let resizedFile = new File([v], file.name);
            this.uploadFiles(resizedFile);
          },     // nextHandler

        });


    }
  }

  uploadFiles(data: any) {
    if (data != undefined && this.FileTypeId != undefined) {
      this.attachmentService.upload(data, this.FileTypeId)
        .pipe()
        .subscribe({

          error: () => { this.loading = false; },    // errorHandler 
          next: (v: any) => {
            this.PushIntoList(v);
            this.fileName = "";
            this.loading = false;
          },     // nextHandler

        });
    }
  }

  PushIntoList(data: any) {
    if (data.fileContent != null) {
      this.filesList.push(data);
      this.attachment.emit(data);// return file details to parent component
    }
  }

  downloadFile(file: any) {
    this.loadingDownload = true;
    this.attachmentService.downloadFile(file.id).subscribe(
      (result: any) => {
        if (result.type != "text/plain") {
          var blob = new Blob([result]);
          saveAs(blob, file.fileName);
          this.loadingDownload = false;
        } else {
          this.loadingDownload = false;
          console.error("error in downloading file");
        }
      },
      (error) => {
        console.log(error);
        this.loadingDownload = false;
      }
    );

  }
  DeleteFile(Id: number, index: number) {
    this.loadingDelete = true;
    this.attachmentService
      .Delete(Id)
      .pipe()
      .subscribe(
        (data) => {
          this.filesList.splice(index, 1);
          this.loadingDelete = false;
        },
        (error) => {
          console.log(error);
          this.loadingDelete = false;
        }
      );
  }

  checkExtension(file: any) {
    var ext = file.name.split(".").pop();
    if (this.allowedExtentions.length > 0) {
      return this.allowedExtentions.includes(ext.toLowerCase());
    }
    return true;

  }



  validateFiles(element: any) {
    if (this.isSingle == true && this.filesList.length > 0) {
      this.fileName = "";
      element.value = null;
      this.errorMessage =
        "Only one file is allowed to upload ,kindly delete previous uploaded file";
      return false;
    }
    if (!this.checkExtension(this.data)) {
      this.errorMessage = `Only ${this.allowedExtentions} are allowed`;
      this.fileName = "";
      element.value = null;
      return false;
    }
    if (this.resizeFile && (this.resizeInWidth == 0 || this.resizeInHeight == 0)) {
      this.errorMessage = `File resize width and height must be provided`;
      this.fileName = "";
      element.value = null;
      return false;

    }
    return true;


  }

}



