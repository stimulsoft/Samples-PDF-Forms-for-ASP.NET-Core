import { Component, ElementRef, ViewChild } from '@angular/core';
import { StimulsoftFormsComponent, StimulsoftFormsService } from 'stimulsoft-forms';
import { StiForm } from 'stimulsoft-forms/lib/elements/StiForm';
import { StiInterfaceEvent } from 'stimulsoft-forms/lib/services/objects';

@Component({
  selector: 'app-root',
  template: `
    <stimulsoft-forms
      #fromComponent
      [requestUrl]="'http://localhost:59904/Forms/Action'"
      [properties]="properties"
      [form]="form"
      [style.width]="'100%'"
      [style.height]="'100%'"
      (interfaceEvent)="interfaceEvent($event)"
      id="sti-form"
    >
    </stimulsoft-forms>
  `
})
export class AppComponent {

  public properties = {};
  public form!: any;

  private formName = "Order.mrt";

  constructor(public formService: StimulsoftFormsService) {
    this.properties = { formName: this.formName, localization: "en" };
  }

  interfaceEvent(event: StiInterfaceEvent) {
    switch (event.name) {
      case "FormNew":
        this.form = this.formService.createElement("Form");
        break;

      case "FormSave":
      case "FormSaveAs":
        this.formService.postData({ action: "FormSave", formName: event.data.name ?? this.formName, form: this.formService.base64Encode(event.data.form.saveToReportJsonObject().serialize()) }, (data: any) => {
          console.log(data);
        });
        break;

      case "FormOpen":
        this.callOpenDialog();
        break;

      case "Loaded":
        let form: StiForm = this.formService.createElement("Form");
        if (event.data.form) {
          form.loadFormJsonString(this.formService.base64Decode(event.data.form));
        }
        this.form = form;
        break;
    }
  }

  public callOpenDialog() {
    let input: any = document.createElement("input");
    input.type = "file";
    let this_ = this;
    input.onchange = () => {
      let file: any = Array.from(input.files)[0];
      this_.formName = input.files[0].name;
      var reader = new FileReader();
      reader.readAsText(file, "UTF-8");
      reader.onload = function (evt: any) {
        let form: StiForm = this_.formService.createElement("Form");
        form.loadFormJsonString(evt.target.result as string);
        this_.form = form;
      };
    };
    input.click();
  }


}
