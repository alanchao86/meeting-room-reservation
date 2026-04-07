import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule],
  template: `<p class="feedback fade-up" [class.success]="tone === 'success'" [class.error]="tone === 'error'">{{ message }}</p>`
})
export class FeedbackComponent {
  @Input({ required: true }) tone: 'success' | 'error' = 'success';
  @Input({ required: true }) message = '';
}
