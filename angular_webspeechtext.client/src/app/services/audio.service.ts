import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

//глобальная переменная для использования Recorder.js
declare var Recorder: any;

@Injectable({
  providedIn: 'root'
})
export class AudioService {
  public micAvailable = false; // доступ к микрофону
  public isRecording = false; // состояние записи
  public speechText = ''; // распознанный текст
  public translatedText = ''; // переведённый текст


  private recorder: any; // экземпляр Recorder.js
  private readonly apiUrl = 'http://localhost:5048/api/audio/upload'; 

  constructor(private http: HttpClient) { }

  // инициализация Recorder.js
  initRecorder(stream: MediaStream): void {
    const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    const input = audioContext.createMediaStreamSource(stream);

  // создаем экземпляр Recorder.js
    this.recorder = new Recorder(input, { numChannels: 1 });
    console.log('Recorder initialized successfully');
  }

  // начало записи
  startRecording(): void {
    if (!this.recorder) {
      console.error('Recorder is not initialized');
      return;
    }

    this.recorder.record(); // запуск записи
    console.log('Recording started');
  }

  // остановка записи
  stopRecording(callback: (blob: Blob) => void): void {
    if (!this.recorder) {
      console.error('Recorder is not initialized');
      return;
    }

    console.log('Stopping recording...');
    this.recorder.stop(); // остановим запись
    console.log('Recorder stopped');

    this.recorder.exportWAV((blob: Blob) => {
      console.log('Audio Blob generated:', blob);
      callback(blob);
      this.recorder.clear(); // очистим данные Recorder
      console.log('Recorder cleared');
    });
  }

  // отправка аудио на сервер
  uploadAudio(audioBlob: Blob): Observable<any> {
    if (!audioBlob.size) {
      console.error('Audio blob is empty');
      return throwError(() => new Error('Cannot upload empty audio blob'));
    }

    const formData = new FormData();
    formData.append('audio', audioBlob, 'recording.wav');

    return this.http.post(this.apiUrl, formData).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Error while uploading audio:', error);
        return throwError(() => new Error('Failed to upload audio'));
      })
    );
  }
}
