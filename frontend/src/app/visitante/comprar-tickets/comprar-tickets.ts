import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HotToastService } from '@ngxpert/hot-toast';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { Ticket, Calendar, Clock } from 'lucide-angular';
import { EventoService } from '../../../../services/EventoService';
import { TicketService } from '../../../../services/TicketService';
import { RelojService } from '../../../../services/RelojService';
import Evento from '../../../../models/Evento.model';
import ComprarTicketRequest from '../../../../models/ComprarTicketRequest.model';
import { TipoEntrada } from '../../../../models/Ticket.model';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-comprar-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, ZardButtonComponent, ZardIconComponent],
  templateUrl: './comprar-tickets.html',
  styleUrl: './comprar-tickets.css',
})
export class ComprarTickets implements OnInit {
  TicketIcon = Ticket;
  CalendarIcon = Calendar;
  ClockIcon = Clock;

  eventos: Evento[] = [];
  loadingEventos = false;
  comprando = false;
  fechaSeleccionada: string = '';
  fechaMinima: string = '';

  private readonly eventoService = inject(EventoService);
  private readonly ticketService = inject(TicketService);
  private readonly relojService = inject(RelojService);
  private readonly toast = inject(HotToastService);
  private readonly router = inject(Router);

  ngOnInit() {
    this.cargarFechaHoraActual();
    this.cargarEventos();
  }

  cargarFechaHoraActual() {
    this.relojService.obtenerFechaHoraActual().subscribe({
      next: (reloj) => {
        this.fechaMinima = reloj.fechaHora.split('T')[0];
        this.fechaSeleccionada = this.fechaMinima;
      },
      error: () => {
        const hoy = new Date();
        this.fechaMinima = hoy.toISOString().split('T')[0];
        this.fechaSeleccionada = this.fechaMinima;
      },
    });
  }

  cargarEventos() {
    this.loadingEventos = true;
    this.eventoService
      .getAll()
      .pipe(finalize(() => (this.loadingEventos = false)))
      .subscribe({
        next: (data) => (this.eventos = data),
        error: () => this.toast.error('Error al cargar eventos'),
      });
  }

  comprarGeneral() {
    if (!this.fechaSeleccionada) {
      this.toast.info('Selecciona una fecha de visita');
      return;
    }

    this.comprando = true;

    const request: ComprarTicketRequest = {
      FechaVisita: `${this.fechaSeleccionada}T00:00`,
      TipoEntrada: 'General',
    };

    this.ticketService
      .comprarTicket(request)
      .pipe(finalize(() => (this.comprando = false)))
      .subscribe({
        next: () => {
          this.toast.success('Entrada general adquirida exitosamente');
          this.router.navigate(['/visitante/perfil']);
        },
        error: (err) => {
          this.toast.error(err.error?.message || 'Error al comprar entrada');
        },
      });
  }

  comprarEvento(eventoId: string) {
    const evento = this.eventos.find((e) => e.id === eventoId);
    if (!evento) return;

    this.comprando = true;

    const request: ComprarTicketRequest = {
      FechaVisita: `${evento.fecha}`,
      TipoEntrada: 'EventoEspecial',
      EventoId: eventoId,
    };

    this.ticketService
      .comprarTicket(request)
      .pipe(finalize(() => (this.comprando = false)))
      .subscribe({
        next: (res) => {
          console.log(res);
          this.toast.success(
            'Ticket para evento adquirido exitosamente, Ticket ID: ' + res.ticketId
          );
          this.router.navigate(['/visitante/perfil']);
        },
        error: (err) => {
          this.toast.error(err.error?.message || 'Error al comprar ticket');
        },
      });
  }
}
