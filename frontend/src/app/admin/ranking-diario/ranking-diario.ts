import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import RankingDiarioResponse from '../../../../models/RankingDiarioResponse.model';
import { RankingService } from '../../../../services/RankingService';
import { inject } from '@angular/core';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardTableComponent } from '@shared/components/table/table.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ranking-diario',
  imports: [ZardCardComponent, ZardTableComponent, CommonModule],
  templateUrl: './ranking-diario.html',
  styleUrl: './ranking-diario.css',
})
export class RankingDiario implements OnInit {
  ranking: RankingDiarioResponse[] = [];
  loading = true;

  private rankingService = inject(RankingService);

  ngOnInit() {
    this.rankingService.getRankingDiario().subscribe({
      next: (data) => {
        this.ranking = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }
}
