import { Component, OnInit } from '@angular/core';
import { AudioService } from './services/audio.service';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  public micAvailable = false; // доступ к микрофону
  public isRecording = false; // состояние записи
  public speechText = ''; // распознанный текст
  public translatedText = ''; // переведённый текст

  constructor(
    public authService: AuthService,
    private audioService: AudioService,
    private router: Router
  ) { }

  ngOnInit(): void {
    console.log('AppComponent initialized');
  }

  // запрос доступа к микрофону
  requestMicrophoneAccess(): void {
    navigator.mediaDevices
      .getUserMedia({ audio: true })
      .then((stream) => {
        this.micAvailable = true;
        this.audioService.initRecorder(stream); // инициализация Recorder.js через сервис
        console.log('Microphone access granted');
      })
      .catch((error) => {
        console.error('Error accessing microphone:', error);
        this.micAvailable = false;
      });
  }

  // отмена доступа к микрофону
  cancelMicrophoneAccess(): void {
    this.micAvailable = false;
    console.log('Microphone access canceled');
  }

  // начало записи
  startRecording(): void {
    if (!this.micAvailable) {
      console.error('Cannot start recording: Microphone access not granted');
      return;
    }

    if (this.isRecording) {
      console.warn('Recording is already in progress');
      return;
    }

    this.isRecording = true;
    this.audioService.startRecording();
    console.log('Recording started');
  }

  // остановка записи
  stopRecording(): void {
    if (!this.isRecording) {
      console.error('Cannot stop recording: No recording in progress');
      return;
    }

    this.isRecording = false;
    this.audioService.stopRecording((audioBlob: Blob) => {
      if (!audioBlob || !audioBlob.size) {
        console.error('No audio recorded to process');
        return;
      }

      console.log('Recording stopped and audio blob generated:', audioBlob);

      this.uploadAndProcessAudio(audioBlob);
    });
  }

  // метод для загрузки аудио и обработки ответа
  private uploadAndProcessAudio(audioBlob: Blob): void {
    this.audioService.uploadAudio(audioBlob).subscribe(
      (response) => {
        console.log('Audio uploaded successfully:', response);

        // обновляем текстовые поля
        this.speechText = response.text || 'No transcription available';
        this.translatedText = response.translatedText || 'No translation available';
      },
      (error) => {
        console.error('Error uploading audio:', error);
        alert('Failed to process audio. Please try again.');
      }
    );
  }

  // выход пользователя
  logout(): void {
    this.authService.logout();
    console.log('Logged out');
  }

  //отображение ссылок авторизации
  shouldShowAuthLinks(): boolean {
    const currentRoute = this.router.url;
    return !this.authService.isLoggedIn() && !['/audio', '/speech'].includes(currentRoute);
  }

}


