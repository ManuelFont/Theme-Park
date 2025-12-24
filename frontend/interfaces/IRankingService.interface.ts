import { Observable } from 'rxjs';
import RankingDiarioResponse from '../models/RankingDiarioResponse.model';

export default interface IRankingService {
  getRankingDiario(): Observable<RankingDiarioResponse[]>;
}
