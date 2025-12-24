import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrearAtraccion } from './crear-atraccion';

describe('CrearAtraccion', () => {
  let component: CrearAtraccion;
  let fixture: ComponentFixture<CrearAtraccion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrearAtraccion]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrearAtraccion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
