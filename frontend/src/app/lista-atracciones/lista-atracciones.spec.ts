import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaAtracciones } from './lista-atracciones';

describe('ListaAtracciones', () => {
  let component: ListaAtracciones;
  let fixture: ComponentFixture<ListaAtracciones>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaAtracciones]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaAtracciones);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
