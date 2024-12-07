using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using System.Text;
using Newtonsoft.Json;
using Angular_WebSpeechText.Models;

//контроллер выполняет следующие функции:

//Принимает загруженный аудиофайл.
//Конвертирует аудиофайл в нужный формат.
//Использует Microsoft Cognitive Services для распознавания речи.
//Переводит текст с помощью Microsoft Translator.
//Отправляет результат обратно клиенту.

namespace Angular_WebSpeechText.Server.Controllers
{  
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private void ConvertToPcm16Le(string inputFilePath, string outputFilePath)
        {
            using (var reader = new AudioFileReader(inputFilePath))
            {
                var format = new WaveFormat(16000, 16, 1);
                using (var resampler = new MediaFoundationResampler(reader, format))
                {
                    WaveFileWriter.CreateWaveFile(outputFilePath, resampler);
                }
            }
        }

        private bool IsWavFile(string filePath)
        {
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                var riff = new string(reader.ReadChars(4));
                var chunkSize = reader.ReadInt32();
                var wave = new string(reader.ReadChars(4));

                return riff == "RIFF" && wave == "WAVE";
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudio()
        {
            var speechConfig = SpeechConfig.FromSubscription("КЛЮЧ АЖУР", "francecentral");

            speechConfig.SpeechRecognitionLanguage = "ru-RUS";

            try
            {
                var audio = Request.Form.Files["audio"];
                if (audio != null)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var filePath = Path.Combine(uploadsPath, "recording.wav");
                    var convertedFilePath = Path.Combine(uploadsPath, "recording_converted.wav");

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await audio.CopyToAsync(stream);
                        }

                        if (!System.IO.File.Exists(filePath))
                        {
                            throw new FileNotFoundException($"File {filePath} was not found after copying.");
                        }

                        var fileInfo = new FileInfo(filePath);
                        if (fileInfo.Length == 0)
                        {
                            throw new Exception($"File {filePath} is empty after copying.");
                        }

                        if (!IsWavFile(filePath))
                        {
                            throw new Exception($"File {filePath} is not a valid WAV file.");
                        }

                        // конвертируем файл в требуемый формат
                        ConvertToPcm16Le(filePath, convertedFilePath);
                        var convertedFileInfo = new FileInfo(convertedFilePath);
                        Console.WriteLine($"Converted file size: {convertedFileInfo.Length}");
                        Console.WriteLine($"Converted file path: {convertedFilePath}");

                        try
                        {
                            using var audioConfig = AudioConfig.FromWavFileInput(convertedFilePath);
                            using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
                            var result = await speechRecognizer.RecognizeOnceAsync();

                            Console.WriteLine($"Recognition Result Text: {result.Text}");
                            // Логирование причины
                            Console.WriteLine($"Recognition Result Reason: {result.Reason}");
                            if (result.Reason == ResultReason.RecognizedSpeech)
                            {
                                Console.WriteLine($"Recognized text: {result.Text}");
                            }
                            else if (result.Reason == ResultReason.NoMatch)
                            {
                                Console.WriteLine("No match found. The speech was not recognized.");
                            }
                            else if (result.Reason == ResultReason.Canceled)
                            {
                                var cancellation = CancellationDetails.FromResult(result);
                                Console.WriteLine($"Recognition canceled. Reason: {cancellation.Reason}");
                                Console.WriteLine($"Error details: {cancellation.ErrorDetails}");
                                // Если причина отмены — ошибка
                                if (cancellation.Reason == CancellationReason.Error)
                                {
                                    Console.WriteLine($"ErrorCode: {cancellation.ErrorCode}");
                                    Console.WriteLine($"ErrorDetails: {cancellation.ErrorDetails}");
                                    Console.WriteLine($"Did you set the correct subscription key and region?");
                                }
                            }

                            string translatedstr = await Translate(result.Text);

                            var obj = JsonConvert.DeserializeObject<Responce[]>(translatedstr);

                            Console.WriteLine($"Recognized text: {result.Text}");
                            Console.WriteLine($"Translated text: {obj[0].translations[0].text}");

                            return Ok(new { Text = result.Text, TranslatedText = obj[0].translations[0].text });
                        }
                        catch (ApplicationException ex)
                        {
                            Console.WriteLine($"Error processing audio file: {ex.Message}");
                            return StatusCode(500, $"Internal server error during audio processing: {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error initializing AudioConfig or SpeechRecognizer: {ex.Message}");
                        Console.WriteLine($"Error writing file to {filePath} or processing audio: {ex.Message}");
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                    finally
                    {
                        // удаление файлов после обработки
                        try
                        {
                            if (filePath != null && System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                                Console.WriteLine($"Deleted file: {filePath}");
                            }

                            if (convertedFilePath != null && System.IO.File.Exists(convertedFilePath))
                            {
                                System.IO.File.Delete(convertedFilePath);
                                Console.WriteLine($"Deleted file: {convertedFilePath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting files: {ex.Message}");
                        }
                    }
                }

                return BadRequest("No audio file uploaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        async static Task<string> Translate(string textToTranslate)
        {
            //translatorTextApiKey
            string key = "КЛЮЧ АЖУР";
            string endpoint = "https://api.cognitive.microsofttranslator.com";
            //                  
            string location = "francecentral";
        // Input and output languages are defined as parameters.
        string route = "/translate?api-version=3.0&from=ru&to=en";

            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
