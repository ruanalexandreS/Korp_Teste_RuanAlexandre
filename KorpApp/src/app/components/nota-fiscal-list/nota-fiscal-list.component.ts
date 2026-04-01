import { Component, OnInit } from '@angular/core';
import { FaturamentoService, NotaFiscal } from '../../services/faturamento.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-nota-fiscal-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  templateUrl: './nota-fiscal-list.component.html',
  styleUrl: './nota-fiscal-list.component.scss'
})
export class NotaFiscalListComponent implements OnInit {
  notas: NotaFiscal[] = [];
  displayedColumns: string[] = ['numeroSequencial', 'status', 'totalItens', 'acoes'];
  loading = false;
  imprimindo: number | null = null;
  erro = '';

  constructor(
    private faturamentoService: FaturamentoService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.carregarNotas();
    this.faturamentoService.refreshNecessario.subscribe(() => this.carregarNotas());
  }

  carregarNotas(): void {
    this.loading = true;
    this.erro = '';
    this.faturamentoService.getNotasFiscais().subscribe({
      next: (data) => {
        this.notas = data;
        this.loading = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar as notas fiscais. Verifique se o ServicoFaturamento está rodando.';
        this.loading = false;
      }
    });
  }

  imprimirNota(id: number): void {
    this.imprimindo = id;
    this.faturamentoService.imprimirNota(id).subscribe({
      next: (resposta) => {
        this.snackBar.open(resposta, 'Fechar', { duration: 4000 });
        this.carregarNotas();
        this.imprimindo = null;
      },
      error: (err: any) => {
        const msg = err?.error?.mensagem || err?.error || 'Erro ao imprimir nota fiscal.';
        this.snackBar.open(`Erro: ${msg}`, 'Fechar', { duration: 6000 });
        console.error(err);
        this.imprimindo = null;
      }
    });
  }
}
