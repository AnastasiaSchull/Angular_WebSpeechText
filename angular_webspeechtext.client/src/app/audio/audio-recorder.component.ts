import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AudioService } from '../services/audio.service';

@Component({
  selector: 'app-audio-recorder',
  templateUrl: './audio-recorder.component.html',
})
export class AudioRecorderComponent {
  micAvailable = false; // флаг доступа к микрофону
  speechText: string = ''; // текст, полученный от сервера
  translatedText: string = ''; // переведённый текст
  isRecording = false; // состояние записи

  constructor(
    public authService: AuthService, 
    private audioService: AudioService
  ) { }

  // запрос доступа к микрофону
  requestMicrophoneAccess(): void {
    navigator.mediaDevices
      .getUserMedia({ audio: true })
      .then((stream) => {
        this.micAvailable = true;
        this.audioService.initRecorder(stream); // инициализация Recorder.js через сервис
        console.log('Microphone access granted and Recorder initialized');
      })
      .catch((error) => {
        console.error('Error accessing microphone:', error);
        this.micAvailable = false;
      });
  }

  // начало записи
  startRecording(): void {
    if (!this.micAvailable) {
      console.error('Cannot start recording: Microphone is not available');
      return;
    }
    this.audioService.startRecording(); // запуск записи через сервис
    this.isRecording = true;
    console.log('Recording started');
  }

  // остановка записи
  stopRecording(): void {
    if (!this.isRecording) {
      console.error('Cannot stop recording: No recording in progress');
      return;
    }

    this.audioService.stopRecording((audioBlob: Blob) => {
      if (!audioBlob || !audioBlob.size) {
        console.error('No audio recorded to process');
        alert('No audio recorded to process. Please try again.');
        return;
      }

      console.log('Recording stopped and audio blob generated:', audioBlob);

      // отправка аудио на сервер
      this.uploadAndProcessAudio(audioBlob);
    });

    this.isRecording = false; // обновление состояния записи
  }

  private uploadAndProcessAudio(audioBlob: Blob): void {
    this.audioService.uploadAudio(audioBlob).subscribe({
      next: (response) => {
        console.log('Audio uploaded successfully:', response);
        this.speechText = response.text || 'No transcription available';
        this.translatedText = response.translatedText || 'No translation available';
      },
      error: (err) => {
        console.error('Error uploading audio:', err);
        alert('Failed to process audio. Please try again.');
      },
    });
  }

  // отключение микрофона
  cancelMicrophoneAccess(): void {
    console.log('Microphone access canceled');
    this.micAvailable = false;
  }

  // выход из приложения
  logout(): void {
    this.authService.logout();
    console.log('Logged out');
  }
}
