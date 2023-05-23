import { Component, ElementRef, ViewChild } from '@angular/core';
import {
  StimulsoftFormsService,
} from 'stimulsoft-forms';
import { StiForm } from 'stimulsoft-forms/lib/elements/StiForm';
import { StiInterfaceEvent } from 'stimulsoft-forms/lib/services/objects';

@Component({
  selector: 'app-root',
  template: `
      <stimulsoft-forms
        #fromComponent
        [requestUrl]="'http://localhost:59903/Forms/Action'"
        [properties]="properties"
        [viewerMode]="true"
        [form]="form"
        [style.width]="'100%'"
        [style.height]="'100%'"
        (interfaceEvent)="interfaceEvent($event)">
      </stimulsoft-forms>
  `
})
export class AppComponent {

  public form!: any;
  public properties = {};

  constructor(public formService: StimulsoftFormsService) {
  }

  interfaceEvent(event: StiInterfaceEvent) {
    switch (event.name) {
      case 'Loaded':
        let form: StiForm = this.formService.createElement('Form');
        form.loadFormJsonString(atob(event.data.form));
        this.form = form;
        break;
      case "FormSave":
        this.downloadFiles(event.data.form.saveToReportJsonObject().serialize(), event.data.name);
        break;
    }
  }

  public downloadFiles(data: any, file_name: string) {
    let file = new Blob([data]);
    let a = document.createElement("a"),
      url = URL.createObjectURL(file);
    a.href = url;
    a.download = file_name;
    document.body.appendChild(a);
    a.click();
    setTimeout(function () {
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }, 0);
  }
}
