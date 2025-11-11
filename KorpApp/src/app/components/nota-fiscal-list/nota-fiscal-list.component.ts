import { Component, OnInit } from '@angular/core';
import { FaturamentoService, NotaFiscal } from '../../services/faturamento.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-nota-fiscal-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './nota-fiscal-list.component.html',
  styleUrl: './nota-fiscal-list.component.scss'
})
export class NotaFiscalListComponent implements OnInit {

  public notas: NotaFiscal[] = [];
  public displayedColumns: string[] = ['numeroSequencial', 'status', 'totalItens', 'acoes'];

  constructor(
    private faturamentoService: FaturamentoService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.carregarNotas();

    // "Ouve" o sinal do formulário para recarregar a lista
    this.faturamentoService.refreshNecessario.subscribe(() => {
      this.carregarNotas();
    });
  }

  carregarNotas(): void {
    this.faturamentoService.getNotasFiscais().subscribe((notasDaApi: NotaFiscal[]) => {
      this.notas = notasDaApi;
    });
  }

  // --- O REQUISITO PRINCIPAL: IMPRIMIR ---
  imprimirNota(id: number): void {
    this.faturamentoService.imprimirNota(id).subscribe({
      next: (resposta) => {
        this.snackBar.open(resposta, 'Fechar', { duration: 3000 });
        this.carregarNotas(); // Recarrega a lista para mostrar o status "Fechada"
      },
      error: (err: any) => {
        // REQUISITO OBRIGATÓRIO: TRATAMENTO DE FALHA!
        // A API (ServicoFaturamento) já nos manda a mensagem de erro tratada.
        this.snackBar.open(`Erro: ${err.error}`, 'Fechar', { duration: 5000 });
        console.error(err);
      }
    });
  }
}