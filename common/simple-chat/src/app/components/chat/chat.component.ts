import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MaterialModule } from '../../material/material.module';
import { FormsModule } from '@angular/forms';
import { ChatHttpService, ChatMessage } from '../../services/chat-http.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, MaterialModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  public message: string = '';
  public chatHistory: ChatMessage[] = [];

  constructor(private readonly chatHttpService: ChatHttpService) {
    
  }

  sendMessage() {
    this.chatHttpService.sendMessage('new', this.message).subscribe({
      next: (response) => {
        console.log(response);
        this.chatHistory.push({ message: this.message, role: 'user' });
        this.chatHistory.push(response);
      },
      error: (error: any) => {
        console.error(error);
      }
    });
  }

}
