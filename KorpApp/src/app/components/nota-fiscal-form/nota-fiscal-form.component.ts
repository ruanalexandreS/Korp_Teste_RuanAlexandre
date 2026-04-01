import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { FaturamentoService } from '../../services/faturamento.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-nota-fiscal-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './nota-fiscal-form.component.html',
  styleUrl: './nota-fiscal-form.component.scss'
})
export class NotaFiscalFormComponent implements OnInit {
  notaFiscalForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private faturamentoService: FaturamentoService,
    private snackBar: MatSnackBar
  ) {
    this.notaFiscalForm = this.fb.group({
      itens: this.fb.array([], Validators.required)
    });
  }

  ngOnInit(): void {
    this.adicionarItem();
  }

  get itens(): FormArray {
    return this.notaFiscalForm.get('itens') as FormArray;
  }

  novoItem(): FormGroup {
    return this.fb.group({
      produtoId: ['', Validators.required],
      quantidade: [1, [Validators.required, Validators.min(1)]]
    });
  }

  adicionarItem(): void {
    this.itens.push(this.novoItem());
  }

  removerItem(index: number): void {
    this.itens.removeAt(index);
  }

  onSubmit(): void {
    if (this.notaFiscalForm.invalid) return;
    this.loading = true;
    this.faturamentoService.addNotaFiscal(this.notaFiscalForm.value.itens).subscribe({
      next: () => {
        this.snackBar.open('Nota Fiscal cadastrada com sucesso!', 'Fechar', { duration: 3000 });
        this.faturamentoService.notificarAtualizacao();
        this.itens.clear();
        this.adicionarItem();
        this.loading = false;
      },
      error: (err: any) => {
        const msg = err?.error?.mensagem || err?.error?.title || 'Erro ao cadastrar Nota Fiscal.';
        this.snackBar.open(msg, 'Fechar', { duration: 5000 });
        console.error(err);
        this.loading = false;
      }
    });
  }
}
