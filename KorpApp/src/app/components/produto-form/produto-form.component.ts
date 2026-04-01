import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EstoqueService, Produto } from '../../services/estoque.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-produto-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './produto-form.component.html',
  styleUrl: './produto-form.component.scss'
})
export class ProdutoFormComponent {
  produtoForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private estoqueService: EstoqueService,
    private snackBar: MatSnackBar
  ) {
    this.produtoForm = this.fb.group({
      codigo: ['', Validators.required],
      descricao: ['', Validators.required],
      saldo: [0, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.produtoForm.invalid) return;
    this.loading = true;
    this.estoqueService.addProduto(this.produtoForm.value).subscribe({
      next: (_: Produto) => {
        this.snackBar.open('Produto cadastrado com sucesso!', 'Fechar', { duration: 3000 });
        this.estoqueService.notificarAtualizacao();
        this.produtoForm.reset({ saldo: 0 });
        this.loading = false;
      },
      error: (err: any) => {
        const msg = err?.error?.title || err?.message || 'Erro ao cadastrar produto.';
        this.snackBar.open(msg, 'Fechar', { duration: 5000 });
        console.error(err);
        this.loading = false;
      }
    });
  }
}
