import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperadorHome } from './operador-home';

describe('OperadorHome', () => {
  let component: OperadorHome;
  let fixture: ComponentFixture<OperadorHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperadorHome]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperadorHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
