using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        Console.Write("Ingrese el ID del post de Reddit: ");
        string postId = Console.ReadLine();

        // Configurar el cliente HTTP con el User-Agent adecuado
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# MyRedditApp/v1.0 (by /u/Jatemas)");

            // Realizar la solicitud a la API de Reddit para obtener los comentarios
            HttpResponseMessage response = await httpClient.GetAsync($"https://api.reddit.com/r/all/comments/{postId}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Respuesta JSON:");
                Console.WriteLine(responseBody);

                // Procesar y mostrar los comentarios mejor votados
                var comments = ParseComments(responseBody)
                    .OrderByDescending(comment => comment.Votes)
                    .Take(10);

                foreach (var comment in comments)
                {
                    Console.WriteLine($"Votos: {comment.Votes}");
                    Console.WriteLine($"Comentario: {comment.Text}");
                    Console.WriteLine("-----------------------");
                }
            }
            else
            {
                Console.WriteLine($"Error en la solicitud: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }

    static IEnumerable<Comment> ParseComments(string responseBody)
    {
        // Implementa la lógica para analizar y extraer los comentarios de la respuesta de la API de Reddit
        // Puedes usar bibliotecas JSON como Newtonsoft.Json para facilitar el análisis

        JObject jsonObject = JObject.Parse(responseBody);

        JArray commentArray = jsonObject["data"]["children"].Value<JArray>();

        foreach (JObject commentJson in commentArray)
        {
            JObject commentDataJson = commentJson["data"].Value<JObject>();
            string text = commentDataJson["body"].ToString();
            int votes = commentDataJson["ups"].ToObject<int>();
            yield return new Comment { Text = text, Votes = votes };
        }
    }
}

class Comment
{
    public string? Text { get; set; }
    public int Votes { get; set; }
}
