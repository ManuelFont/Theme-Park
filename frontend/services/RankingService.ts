import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import RankingDiarioResponse from '../models/RankingDiarioResponse.model';
import { environment } from '../src/environments/environment.development';
import IRankingService from '../interfaces/IRankingService.interface';

@Injectable({
  providedIn: 'root',
})
export class RankingService implements IRankingService {
  private readonly apiUrl = `${environment.apiUrl}/ranking`;

  private http = inject(HttpClient);

  getRankingDiario(): Observable<RankingDiarioResponse[]> {
    return this.http.get<RankingDiarioResponse[]>(`${this.apiUrl}/diario?top=${top}`);
  }
}
