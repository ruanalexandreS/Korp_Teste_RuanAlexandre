import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';

//  Para fazer chamadas de API
import { HttpClientModule } from '@angular/common/http'; 
//  Para usar formulários
import { ReactiveFormsModule } from '@angular/forms'; 
//  Módulos do Angular Material
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSnackBarModule } from '@angular/material/snack-bar';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(),
    provideAnimations(), // Para as animações do Material

    importProvidersFrom(
      HttpClientModule,     // Permite fazer chamadas de API
      ReactiveFormsModule,  // Permite criar formulários
      
      // Módulos do Material
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule,
      MatTableModule,
      MatCardModule,
      MatToolbarModule,
      MatSnackBarModule
    )
  ]
};