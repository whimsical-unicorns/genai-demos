import { Component } from '@angular/core';
import { RouterLinkActive, RouterModule, RouterOutlet } from '@angular/router';
import { MaterialModule } from './material/material.module';
import { ChatComponent } from "./components/chat/chat.component";
import { ApiSelectionService } from './services/api-selection.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    imports: [
      RouterOutlet, RouterModule, RouterLinkActive, CommonModule,
      MaterialModule,
      ChatComponent
    ]
})
export class AppComponent {
  title = 'simple-chat';
  public api$ = this.apiSelectionService.api$;

  constructor(private apiSelectionService: ApiSelectionService) {
  }

  public setApi(api: string): void {
    this.apiSelectionService.api = api;
  }
}
