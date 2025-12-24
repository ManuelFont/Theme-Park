import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Egreso } from './egreso';

describe('Egreso', () => {
  let component: Egreso;
  let fixture: ComponentFixture<Egreso>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Egreso]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Egreso);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
