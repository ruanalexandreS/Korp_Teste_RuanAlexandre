import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { ProdutoFormComponent } from './components/produto-form/produto-form.component';
import { ProdutoListComponent } from './components/produto-list/produto-list.component';
import { NotaFiscalFormComponent } from './components/nota-fiscal-form/nota-fiscal-form.component';
import { NotaFiscalListComponent } from './components/nota-fiscal-list/nota-fiscal-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    MatToolbarModule,
    MatTabsModule,
    MatIconModule,
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
