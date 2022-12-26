import { Component, ElementRef, ViewChild } from '@angular/core';
import { StimulsoftFormsComponent, StimulsoftFormsService } from 'stimulsoft-forms';
import { StiForm } from 'stimulsoft-forms/lib/elements/StiForm';
import { StiInterfaceEvent } from 'stimulsoft-forms/lib/services/objects';

@Component({
  selector: 'app-root',
  template: `
  <div style="height:30px; width:100vw; text-align:center">
     <input type="button" value="Online form" style="margin:3px" (click)="mode='online-form'"/>
     <input type="button" value="PDF form" style="margin:3px" (click)="mode='pdf-form'"/>
     <input type="button" value="PDF form submit result as PDF" style="margin:3px" (click)="mode='pdf-form-as-pdf'"/>
     <input type="button" value="Results" style="margin:3px" (click)="mode='results'"/>
  </div>
  <div style="position: absolute;top:30px;bottom:0px;left:0; right:0">
    <stimulsoft-forms *ngIf="mode=='online-form'"
      #fromComponent
      [requestUrl]="'http://localhost:9429/Forms/Action'"
      [properties]="properties"
      [viewerMode]="true"
      [form]="form"
      (interfaceEvent)="interfaceEvent($event)">
    </stimulsoft-forms>
    <embed *ngIf="mode=='pdf-form'"
        src="http://localhost:9429/Forms/Pdf?name=Order.mrt"
        type="application/pdf"
        width="100%"
        height="100%" />
    <div *ngIf="mode=='pdf-form-as-pdf'">
        Submit as PDF allowed only in Acrobat Reader, please <a href="http://localhost:9429/Forms/Pdf?name=Order.mrt&submit=pdf" download="Order.pdf">download</a> file & open it in Acrobat Reader.
    </div>
    <iframe *ngIf="mode=='results'"
        src="http://localhost:9429/Forms/Results?name=Order.mrt"
        style="width: 100%; height: 100%">
    </iframe>
  </div>
  `,
  styles: [
    `
      :host {
        height: 100%;
        width: 100%;
      }
      stimulsoft-forms {
        height: 90%;
      }
    `,
  ],
})
export class AppComponent {

  public properties = {};
  public form!: any;
  public mode = "online-form"

  private formName = "Order.mrt";

  constructor(public formService: StimulsoftFormsService) {
    this.properties = { formName: this.formName };
  }

  interfaceEvent(event: StiInterfaceEvent) {
    switch (event.name) {
      case "Loaded":
        let form: StiForm = this.formService.createElement("Form");
        form.loadFormJsonString(atob(event.data.form));
        this.form = form;
        break;
      case "FormSubmitted":
        alert(event.data.message);
        break;
      case "FormSubmittedError":
        alert(event.data);
        break;
    }
  }


}
