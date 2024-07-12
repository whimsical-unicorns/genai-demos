import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiSelectionService } from './api-selection.service';

export interface ChatMessage {
  message: string;
  role?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatHttpService {
  constructor(private readonly httpClient: HttpClient, private readonly apiSelectionService: ApiSelectionService) {
  }

  sendMessage(sessionId: string, message: string): Observable<ChatMessage> {
    return this.httpClient.post<ChatMessage>(`${this.apiSelectionService.api}/v1/chat/sessions/${sessionId}`, { prompt: message });
  }
}
