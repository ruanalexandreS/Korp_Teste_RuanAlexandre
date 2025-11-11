import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotaFiscalForm } from './nota-fiscal-form.component';

describe('NotaFiscalForm', () => {
  let component: NotaFiscalForm;
  let fixture: ComponentFixture<NotaFiscalForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotaFiscalForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
