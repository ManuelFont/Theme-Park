import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitanteHome } from './visitante-home';

describe('VisitanteHome', () => {
  let component: VisitanteHome;
  let fixture: ComponentFixture<VisitanteHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VisitanteHome]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VisitanteHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
