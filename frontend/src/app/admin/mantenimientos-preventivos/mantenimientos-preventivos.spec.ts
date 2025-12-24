import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientosPreventivos } from './mantenimientos-preventivos';

describe('MantenimientosPreventivos', () => {
  let component: MantenimientosPreventivos;
  let fixture: ComponentFixture<MantenimientosPreventivos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientosPreventivos]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientosPreventivos);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
