import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Atraccion } from './atraccion';

describe('Atraccion', () => {
  let component: Atraccion;
  let fixture: ComponentFixture<Atraccion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Atraccion]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Atraccion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
