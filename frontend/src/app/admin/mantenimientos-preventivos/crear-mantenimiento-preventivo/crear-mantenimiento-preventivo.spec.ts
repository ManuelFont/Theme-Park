import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrearMantenimientoPreventivo } from './crear-mantenimiento-preventivo';

describe('CrearMantenimientoPreventivo', () => {
  let component: CrearMantenimientoPreventivo;
  let fixture: ComponentFixture<CrearMantenimientoPreventivo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrearMantenimientoPreventivo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrearMantenimientoPreventivo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
