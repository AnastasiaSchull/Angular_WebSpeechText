import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { AudioService } from './services/audio.service';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;
  let audioServiceSpy: jasmine.SpyObj<AudioService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('AudioService', ['init']);

    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule],
      providers: [
        { provide: AudioService, useValue: spy }
      ]
    }).compileComponents();
    audioServiceSpy = TestBed.inject(AudioService) as jasmine.SpyObj<AudioService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize microphone on start', () => {
    const mockStream = new MediaStream();
    navigator.mediaDevices.getUserMedia = jasmine.createSpy().and.returnValue(Promise.resolve(mockStream));

    fixture.detectChanges();

    expect(navigator.mediaDevices.getUserMedia).toHaveBeenCalledWith({ audio: true });
    expect(audioServiceSpy.initRecorder).toHaveBeenCalledWith(mockStream);
  });
});
