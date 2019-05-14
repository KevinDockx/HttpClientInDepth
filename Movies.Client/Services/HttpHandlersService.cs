using Marvin.StreamExtensions;
using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class HttpHandlersService : IIntegrationService
    { 
        private CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private HttpClient _httpClient =
           new HttpClient(
               new TimeOutDelegatingHandler(
                    new HttpClientHandler()
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.GZip
                    },
                    new TimeSpan(0,0,2)
               ));


        public HttpHandlersService()
        {
            // set up HttpClient instance
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            await GetMovies(_cancellationTokenSource.Token);
        }

        public async Task GetMovies(CancellationToken cancellationToken)
        {           
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {                
                response.EnsureSuccessStatusCode();                

                var stream = await response.Content.ReadAsStreamAsync();
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
        }
    }
}
