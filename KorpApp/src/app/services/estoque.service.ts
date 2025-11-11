import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';

export interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  saldo: number;
}

@Injectable({
  providedIn: 'root'
})
export class EstoqueService {

  private apiUrl = 'https://localhost:7296/api/produtos';

  // Um Subject privado que vai "disparar" o sinal
  private _refreshNecessario$ = new Subject<void>();

  get refreshNecessario() {
    return this._refreshNecessario$;
  }

  // Injetamos o HttpClient que configuramos no app.config.ts
  constructor(private http: HttpClient) { }

  // GET: api/produtos
  getProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }

  // POST: api/produtos
  addProduto(produto: Omit<Produto, 'id'>): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, produto);
  }

  // O formulário vai chamar este método depois do sucesso
  notificarAtualizacao() {
    this._refreshNecessario$.next();
  }
}