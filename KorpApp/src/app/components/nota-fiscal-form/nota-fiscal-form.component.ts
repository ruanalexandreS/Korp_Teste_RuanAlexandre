import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { FaturamentoService } from '../../services/faturamento.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ReactiveFormsModule } from '@angular/forms';
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
    MatIconModule
  ],
  templateUrl: './nota-fiscal-form.component.html',
  styleUrl: './nota-fiscal-form.component.scss'
})
export class NotaFiscalFormComponent implements OnInit {

  notaFiscalForm: FormGroup;

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
    if (this.notaFiscalForm.valid) {
      this.faturamentoService.addNotaFiscal(this.notaFiscalForm.value.itens).subscribe({
        next: (nota) => {
          this.snackBar.open('Nota Fiscal cadastrada com sucesso!', 'Fechar', { duration: 3000 });
          
          this.faturamentoService.notificarAtualizacao();
          
          this.itens.clear();
          this.adicionarItem();
        },
        error: (err: any) => {
          this.snackBar.open('Erro ao cadastrar Nota Fiscal.', 'Fechar', { duration: 3000 });
          console.error(err);
        }
      });
    }
  }
}