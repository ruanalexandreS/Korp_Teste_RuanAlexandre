import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

export interface ItemNotaFiscal {
  id: number;
  produtoId: number;
  quantidade: number;
  notaFiscalId: number;
}

export interface NotaFiscal {
  id: number;
  numeroSequencial: number;
  status: string;
  itens: ItemNotaFiscal[];
}

@Injectable({
  providedIn: 'root'
})
export class FaturamentoService {

  // ATENÇÃO: Verifique a porta do seu ServicoFaturamento!
  private apiUrl = 'https://localhost:7103/api/notasfiscais';

  private _refreshNecessario$ = new Subject<void>();
  get refreshNecessario() {
    return this._refreshNecessario$;
  }

  constructor(private http: HttpClient) { }

  // GET: api/notasfiscais/listar
  getNotasFiscais(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(`${this.apiUrl}/listar`);
  }

  // POST: api/notasfiscais
  addNotaFiscal(itens: { produtoId: number, quantidade: number }[]): Observable<NotaFiscal> {
    
    const novaNota = {
      itens: itens
    };
    
    return this.http.post<NotaFiscal>(this.apiUrl, novaNota);
  }

  // POST: api/notasfiscais/{id}/imprimir
  imprimirNota(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/imprimir`, {}, { responseType: 'text' });
  }

  // Método para "disparar" o sinal
  notificarAtualizacao() {
    this._refreshNecessario$.next();
  }
}