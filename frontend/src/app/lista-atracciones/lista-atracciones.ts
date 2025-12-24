import { Component } from '@angular/core';
import { Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import Atraccion from '../../../models/Atraccion/Atraccion.model';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { LucideAngularModule, SquarePen, TrashIcon } from 'lucide-angular';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'lista-atracciones',
  standalone: true,
  imports: [ZardButtonComponent, ZardIconComponent, LucideAngularModule, CommonModule],
  templateUrl: './lista-atracciones.html',
  styleUrl: './lista-atracciones.css',
})
export class ListaAtracciones {
  TrashIcon = TrashIcon;
  SquarePen = SquarePen;
  @Input() atracciones: Atraccion[] = [];
  @Input() modo: 'admin' | 'operador' = 'admin';

  @Output() crear = new EventEmitter<void>();
  @Output() editar = new EventEmitter<Atraccion>();
  @Output() eliminar = new EventEmitter<string>();
  @Output() seleccionar = new EventEmitter<Atraccion>();

  onCrear() {
    this.crear.emit();
  }

  onEditar(a: Atraccion) {
    this.editar.emit(a);
  }

  onEliminar(id: string) {
    this.eliminar.emit(id);
  }

  onSeleccionar(a: Atraccion) {
    this.seleccionar.emit(a);
  }
}
