import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RankingDiario } from './ranking-diario';

describe('RankingDiario', () => {
  let component: RankingDiario;
  let fixture: ComponentFixture<RankingDiario>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RankingDiario]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RankingDiario);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
