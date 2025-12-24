import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import Atraccion from '../../../../../models/Atraccion/Atraccion.model';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { ZardInputDirective } from '@shared/components/input/input.directive';
import { ZardRadioComponent } from '@shared/components/radio/radio.component';
import { ArrowLeft, CirclePlus, Eye, EyeOff, UserPlus } from 'lucide-angular';
import AtraccionFormValue from '../../../../../models/Atraccion/AtraccionFormValue.model';
import { HotToastService } from '@ngxpert/hot-toast';

@Component({
  selector: 'app-atraccion-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardButtonComponent,
    ZardRadioComponent,
    ZardCardComponent,
    ZardButtonComponent,
    ZardInputDirective,
    ZardIconComponent,
    RouterLink,
  ],
  templateUrl: './atraccion-form.html',
  styleUrl: './atraccion-form.css',
})
export class AtraccionForm implements OnInit, OnChanges {
  @Input() mode: 'crear' | 'editar' = 'crear';
  @Input() loading = false;
  @Input() initialData?: Atraccion | null;
  @Output() submitForm = new EventEmitter<AtraccionFormValue>();

  private readonly fb = inject(FormBuilder);
  private toast = inject(HotToastService);
  CirclePlus = CirclePlus;

  form: FormGroup = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    tipo: ['', Validators.required],
    edadMinima: [0, [Validators.required, Validators.min(0)]],
    capacidadMaxima: [1, [Validators.required, Validators.min(1)]],
    descripcion: ['', [Validators.required, Validators.minLength(5)]],
    disponible: [true, Validators.required],
  });

  hidePassword = true;
  hideConfirm = true;
  iconEye = Eye;
  iconEyeOff = EyeOff;
  UserPlus = UserPlus;
  ArrowLeft = ArrowLeft;

  ngOnInit(): void {
    if (this.mode === 'editar') {
      // cosas
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialData']?.currentValue) {
      setTimeout(() => {
        this.patchAtraccionData(changes['initialData'].currentValue);
      });
    }
  }

  get f() {
    return this.form.controls;
  }

  private patchAtraccionData(atraccion: Atraccion) {
    const cleanData = {
      ...atraccion,
    };
    this.form.patchValue(cleanData);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.info('Debes completar todos los campos');
      return;
    }

    this.submitForm.emit(this.form.value);
  }
}
