import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  inject,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { ZardInputDirective } from '@shared/components/input/input.directive';
import { ZardCardComponent } from '@shared/components/card/card.component';
import { ZardButtonComponent } from '@shared/components/button/button.component';
import { ZardIconComponent } from '@shared/components/icon/icon.component';
import { LucideAngularModule, UserPlus, ArrowLeft, Eye, EyeOff } from 'lucide-angular';
import { RouterLink } from '@angular/router';
import Usuario from '../../../../../models/Usuario/Usuario.model';
import { HotToastService } from '@ngxpert/hot-toast';
import { UsuarioFormOutput } from '../../../../../models/Usuario/UsuarioFormOutput.model';

@Component({
  selector: 'app-user-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ZardInputDirective,
    ZardCardComponent,
    ZardButtonComponent,
    ZardIconComponent,
    LucideAngularModule,
    RouterLink,
  ],
  templateUrl: './user-form.html',
  styleUrl: './user-form.css',
})
export class UserForm implements OnInit, OnChanges {
  @Input() mode: 'crear' | 'editar' = 'crear';
  @Input() loading = false;
  @Input() initialData: Usuario | null = null;
  @Output() submitForm = new EventEmitter<UsuarioFormOutput>();

  private readonly fb = inject(FormBuilder);
  private toast = inject(HotToastService);

  form: FormGroup = this.fb.group(
    {
      nombre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      apellido: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      contrasenia: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern(/^(?=.*[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]).+$/),
        ],
      ],
      confirmarContrasenia: ['', Validators.required],
      tipoUsuario: ['', Validators.required],
      fechaNacimiento: [null],
      nivelMembresia: [''],
    },
    { validator: this.matchPasswords }
  );

  hidePassword = true;
  hideConfirm = true;
  iconEye = Eye;
  iconEyeOff = EyeOff;
  UserPlus = UserPlus;
  ArrowLeft = ArrowLeft;

  ngOnInit(): void {
    if (this.mode === 'editar') {
      this.form.get('contrasenia')?.clearValidators();
      this.form.get('confirmarContrasenia')?.clearValidators();
      this.form.updateValueAndValidity();
    }

    this.form.get('tipoUsuario')?.valueChanges.subscribe((tipo) => {
      const fechaCtrl = this.form.get('fechaNacimiento');
      const nivelCtrl = this.form.get('nivelMembresia');

      if (tipo === 'Visitante') {
        fechaCtrl?.setValidators([Validators.required]);
        nivelCtrl?.setValidators([Validators.required]);
      } else {
        fechaCtrl?.clearValidators();
        nivelCtrl?.clearValidators();

        fechaCtrl?.setValue(null);
        nivelCtrl?.setValue(null);
      }

      fechaCtrl?.updateValueAndValidity();
      nivelCtrl?.updateValueAndValidity();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialData']?.currentValue) {
      setTimeout(() => {
        this.patchUserData(changes['initialData'].currentValue);
      });
    }
  }

  get f() {
    return this.form.controls;
  }

  private patchUserData(user: Usuario) {
    const cleanData = {
      ...user,
      fechaNacimiento: user.fechaNacimiento ? user.fechaNacimiento.split('T')[0] : '',
    };
    this.form.patchValue(cleanData);
  }

  matchPasswords(group: FormGroup) {
    return group.get('contrasenia')?.value === group.get('confirmarContrasenia')?.value
      ? null
      : { mismatch: true };
  }

  onSubmit() {
    console.log(this.form.value);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.info('Debes completar todos los campos');
      return;
    }

    this.submitForm.emit(this.form.value);
  }
}
