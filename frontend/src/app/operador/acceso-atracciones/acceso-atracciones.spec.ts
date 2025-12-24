import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccesoAtracciones } from './acceso-atracciones';

describe('AccesoAtracciones', () => {
  let component: AccesoAtracciones;
  let fixture: ComponentFixture<AccesoAtracciones>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccesoAtracciones]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccesoAtracciones);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
