import { Component, OnInit } from '@angular/core';
import { EstoqueService, Produto } from '../../services/estoque.service'; // Importamos o service
import { CommonModule } from '@angular/common'; // Para *ngFor
import { MatTableModule } from '@angular/material/table'; // Para a tabela

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule
  ],
  templateUrl: './produto-list.component.html',
  styleUrl: './produto-list.component.scss'
})
export class ProdutoListComponent implements OnInit {

  // A lista de produtos que virá da API
  public produtos: Produto[] = [];
  // As colunas que nossa tabela vai mostrar
  public displayedColumns: string[] = ['codigo', 'descricao', 'saldo'];

  constructor(private estoqueService: EstoqueService) { }

  ngOnInit(): void {
    this.carregarProdutos();

    this.estoqueService.refreshNecessario.subscribe(() => {
      this.carregarProdutos();
    });
  }

  carregarProdutos(): void {
    this.estoqueService.getProdutos().subscribe((produtosDaApi: Produto[]) => {
      this.produtos = produtosDaApi;
    });
  }
}