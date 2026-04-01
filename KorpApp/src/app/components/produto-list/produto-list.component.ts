import { Component, OnInit } from '@angular/core';
import { EstoqueService, Produto } from '../../services/estoque.service';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatIconModule,
    MatProgressBarModule,
    MatChipsModule
  ],
  templateUrl: './produto-list.component.html',
  styleUrl: './produto-list.component.scss'
})
export class ProdutoListComponent implements OnInit {
  produtos: Produto[] = [];
  displayedColumns: string[] = ['codigo', 'descricao', 'saldo'];
  loading = false;
  erro = '';

  constructor(private estoqueService: EstoqueService) {}

  ngOnInit(): void {
    this.carregarProdutos();
    this.estoqueService.refreshNecessario.subscribe(() => this.carregarProdutos());
  }

  carregarProdutos(): void {
    this.loading = true;
    this.erro = '';
    this.estoqueService.getProdutos().subscribe({
      next: (data) => {
        this.produtos = data;
        this.loading = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar os produtos. Verifique se o ServicoEstoque está rodando.';
        this.loading = false;
      }
    });
  }

  saldoBaixo(saldo: number): boolean {
    return saldo < 5;
  }
}
