﻿using System.Text.Json;

namespace ProyectoPracticaII.Client.Servicios
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient http;

        public HttpService(HttpClient http)
        {

            this.http = http;
        }

        public async Task<HttpRepuesta<T>> Get<T>(string url)
        {
            var response = await http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var respuesta = await DeserializarRepuesta<T>(response);
                return new HttpRepuesta<T>(respuesta, false, response);
            }
            else
            {
                return new HttpRepuesta<T>(default, true, response);
            }
        }

        private async Task<T> DeserializarRepuesta<T>(HttpResponseMessage response)
        {
            var respuestaStr = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(respuestaStr, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
