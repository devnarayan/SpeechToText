using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToText
{
    class Program
    {
        static void Main(string[] args)
        {
            //EncodeSignleChannel();

               GoogleCredential googleCredential;
            using (Stream m = new FileStream("SpeechToText-e03321616d8f.json", FileMode.Open))
                googleCredential = GoogleCredential.FromStream(m);
            var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            var speech = SpeechClient.Create(channel);

            var config = new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                //SampleRateHertz = 44100,
                //AudioChannelCount=1,
                LanguageCode = LanguageCodes.English.UnitedStates
            };

            var audio = RecognitionAudio.FromFile("channel1.wav");
            var response = speech.Recognize(config, audio);
            foreach(var result in response.Results)
            {
                foreach(var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
        }

       static void EncodeSignleChannel()
        {
            var reader = new WaveFileReader("audit.wav");
            var buffer = new byte[2 * reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
            var writers = new WaveFileWriter[reader.WaveFormat.Channels];
            for (int n = 0; n < writers.Length; n++)
            {
                var format = new WaveFormat(reader.WaveFormat.SampleRate, 16, 1);
                writers[n] = new WaveFileWriter(String.Format("channel{0}.wav", n + 1), format);
            }
            int bytesRead;
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                int offset = 0;
                while (offset < bytesRead)
                {
                    for (int n = 0; n < writers.Length; n++)
                    {
                        // write one sample
                        writers[n].Write(buffer, offset, 2);
                        offset += 2;
                    }
                }
            }
            for (int n = 0; n < writers.Length; n++)
            {
                writers[n].Dispose();
            }
            reader.Dispose();
        }
    }
}
