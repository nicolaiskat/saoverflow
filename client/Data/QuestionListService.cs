using System.Net.Http.Json;
using mylib.Model;

namespace client.Data
{

    public class QuestionListService
    {
        private readonly HttpClient http;
        private readonly IConfiguration configuration;
        private readonly string baseAPI = "";

        public QuestionListService(HttpClient http, IConfiguration configuration)
        {
            this.http = http;
            this.configuration = configuration;
            baseAPI = configuration["base_api"];
        }

        public async Task<Topic[]> GetTopics()
        {
            string url = $"{baseAPI}topics/";
            Console.WriteLine(url);
            return await http.GetFromJsonAsync<Topic[]>(url);
        }

    }
}
