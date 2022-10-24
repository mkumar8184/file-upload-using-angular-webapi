import { Injectable } from "@angular/core";
import { HttpmoduleService } from "../../helpers/httpmodule.service";
import { ApiconstantService } from "../../helpers/httpConstant";

@Injectable({
  providedIn: "root",
})
export class AttachmentService {
  constructor(
    private httpHelper: HttpmoduleService,
    private apiUrl: ApiconstantService
  ) {

  }

  upload(files:any, fileTypeId:any) {
    if (files.length === 0) return null;

    const formData = new FormData();

    formData.append("FileTypeId", fileTypeId);
    formData.append("File", files);
  
    return this.httpHelper.apiPost(
      this.apiUrl.endpoint + "upload-files",
      formData
    );
    
  }
  Delete(id: number) {
    return this.httpHelper.apiDelete(
      this.apiUrl.endpoint  + id,
      null
    );
  }



  downloadFile(fileId: number) {
    return this.httpHelper.apiGetSteam(
      this.apiUrl.endpoint + "downloads-file/" + fileId,
      null
    );
  }


}
