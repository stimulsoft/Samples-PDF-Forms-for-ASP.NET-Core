import { Component } from '@angular/core';
import { StimulsoftFormsService } from 'stimulsoft-forms';

@Component({
  selector: 'app-root',
  template: `
    <stimulsoft-forms
      [requestUrl]="'http://localhost:59906/Forms/Action'"
      [form]="form"
      [style.width]="'100%'"
      [style.height]="'100%'">
    </stimulsoft-forms>
  `
})
export class AppComponent {

  public form!: any;

  constructor(public formService: StimulsoftFormsService) {
    this.form = this.formService.createElement("Form");
  }

}
