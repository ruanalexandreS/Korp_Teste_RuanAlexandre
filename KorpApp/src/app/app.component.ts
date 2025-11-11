import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ProdutoFormComponent } from './components/produto-form/produto-form.component'; 
import { ProdutoListComponent } from './components/produto-list/produto-list.component'; 
import { NotaFiscalFormComponent } from './components/nota-fiscal-form/nota-fiscal-form.component'; 
import { NotaFiscalListComponent } from './components/nota-fiscal-list/nota-fiscal-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    MatToolbarModule,
    ProdutoFormComponent,
    ProdutoListComponent,
    NotaFiscalFormComponent,
    NotaFiscalListComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'KorpApp';
}