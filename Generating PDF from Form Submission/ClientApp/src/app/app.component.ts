import { Component, ElementRef, ViewChild } from '@angular/core';
import {
  StimulsoftFormsService,
} from 'stimulsoft-forms';
import { StiForm } from 'stimulsoft-forms/lib/elements/StiForm';
import { StiInterfaceEvent } from 'stimulsoft-forms/lib/services/objects';
import { StiHelperService } from 'stimulsoft-forms/lib/services/sti-helper.service';

@Component({
  selector: 'app-root',
  template: `
      <div style="width: 100%;height:100%">
        <div style="width: 50%; display: inline-block;height: 100%;">
          <stimulsoft-forms
          [requestUrl]="'http://localhost:59905/Forms/Action'"
          [properties]="{}"
          [viewerMode]="true"
          [form]="form"
          [style.width]="'100%'"
          [style.height]="'100%'"
          (interfaceEvent)="interfaceEvent($event)">
        </stimulsoft-forms>
        </div><div style="width: 50%; height: 100%; display: inline-block;">
            <iframe src="http://localhost:59905/Forms/GetPdf" style="width: 100%; height: 100%" #iframe>
        </iframe>
        </div>
      </div>



  `,
  styles: [
    `

    `,
  ],
})
export class AppComponent {

  @ViewChild("iframe") iframe!: ElementRef;

  public form!: any;
  public properties = {};

  constructor(public formService: StimulsoftFormsService) {
    setInterval(() => {
      this.formService.postData({ action: 'IsDataChanged' }, (data: any) => {
        if (data === true) {
          this.iframe.nativeElement.src = "http://localhost:9429/Forms/GetPdf";
        }
      }, { showProgress: false });
    }, 1500);
  }

  interfaceEvent(event: StiInterfaceEvent) {
    switch (event.name) {
      case 'Loaded':
        let form: StiForm = this.formService.createElement('Form');
        form.loadFormJsonString(this.formService.base64Decode(event.data.form));
        this.form = form;
        break;

      case 'FormSubmitted':
        this.iframe.nativeElement.src = "http://localhost:9429/Forms/GetPdf";
        break;

      case 'FormNew':
        this.form = this.formService.createElement('Form');
        break;

      case 'FormSave':
        this.formService.postData({ action: 'SaveForm', form: this.formService.base64Encode(this.form.saveToReportJsonObject().serialize()) }, () => { });
        break;
    }
  }
}
