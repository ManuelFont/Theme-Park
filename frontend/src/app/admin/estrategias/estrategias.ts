import { Component, inject, OnInit } from '@angular/core';
import Estrategia from '../../../../models/Estrategia/Estrategia.model';
import { EstrategiasService } from '../../../../services/EstrategiasService';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardRadioComponent } from '@shared/components/radio/radio.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-estrategias',
  imports: [ZardCardComponent, ZardRadioComponent, FormsModule],
  templateUrl: './estrategias.html',
  styleUrl: './estrategias.css',
})
export class EstrategiasComponent implements OnInit {
  estrategias: Estrategia[] = [];
  activa: Estrategia | null = null;
  estrategiaSeleccionada: string | null = null;
  cargando = false;
  cambiando = false;

  private estrategiasService = inject(EstrategiasService);

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos() {
    this.cargando = true;

    this.estrategiasService.obtenerTodas().subscribe((lista) => {
      this.estrategias = lista;

      this.estrategiasService.obtenerActiva().subscribe((activa) => {
        this.activa = activa;
        this.cargando = false;
      });
    });
  }

  cambiar() {
    if (!this.estrategiaSeleccionada) return;
    this.cambiando = true;

    this.estrategiasService.cambiar(this.estrategiaSeleccionada).subscribe(() => {
      this.cargarDatos();
      this.cambiando = false;
    });
  }
}
