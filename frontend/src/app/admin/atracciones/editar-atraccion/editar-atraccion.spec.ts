import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditarAtraccion } from './editar-atraccion';

describe('EditarAtraccion', () => {
  let component: EditarAtraccion;
  let fixture: ComponentFixture<EditarAtraccion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditarAtraccion]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditarAtraccion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
