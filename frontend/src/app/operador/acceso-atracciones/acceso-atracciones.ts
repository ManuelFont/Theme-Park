import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OperadorService } from '../../../../services/OperadorService';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';

@Component({
  selector: 'app-acceso-atracciones',
  standalone: true,
  imports: [CommonModule, FormsModule, ZardCardComponent, ZardButtonComponent],
  templateUrl: './acceso-atracciones.html',
})
export class AccesoAtracciones {
  private operadorService = inject(OperadorService);
  private toast = inject(HotToastService);

  atraccionId = '';
  codigoQR = '';
  codigoNFC = '';

  cargando = false;

  limpiarCampos() {
    this.codigoQR = '';
    this.codigoNFC = '';
  }

  registrarIngresoQR() {
    if (!this.codigoQR || !this.atraccionId) {
      this.toast.error('Faltan datos para registrar ingreso por QR');
      return;
    }

    this.cargando = true;

    this.operadorService
      .registrarIngresoPorTicket(this.codigoQR, this.atraccionId)
      .pipe(
        this.toast.observe({
          loading: 'Procesando ingreso...',
          success: 'Ingreso permitido (QR)',
          error: (err: any) => err.error?.message || 'No se pudo registrar el ingreso',
        })
      )
      .subscribe(() => {
        this.cargando = false;
        this.limpiarCampos();
      });
  }

  registrarIngresoNFC() {
    if (!this.codigoNFC || !this.atraccionId) {
      this.toast.error('Faltan datos para registrar ingreso por NFC');
      return;
    }

    this.cargando = true;

    this.operadorService
      .registrarIngresoPorNfc(this.codigoNFC, this.atraccionId)
      .pipe(
        this.toast.observe({
          loading: 'Procesando ingreso...',
          success: 'Ingreso permitido (NFC)',
          error: (err: any) => err.error?.message || 'No se pudo registrar el ingreso',
        })
      )
      .subscribe(() => {
        this.cargando = false;
        this.limpiarCampos();
      });
  }
}
