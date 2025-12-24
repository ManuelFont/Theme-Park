import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporteUsoAtracciones } from './reporte-uso-atracciones';

describe('ReporteUsoAtracciones', () => {
  let component: ReporteUsoAtracciones;
  let fixture: ComponentFixture<ReporteUsoAtracciones>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReporteUsoAtracciones]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReporteUsoAtracciones);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
