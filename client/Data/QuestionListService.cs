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

        // Hent alle topics "/topics"
        public async Task<Topic[]?> GetTopics()
        {
            string url = $"{baseAPI}topics/";
            Console.WriteLine(url);
            return await http.GetFromJsonAsync<Topic[]>(url);
        }

        // Hent enkelte topics "/topics/{id:int}
        public async Task<Topic?> GetTopicById(int id)
        {
            string url = $"{baseAPI}topics/{id}";
            return await http.GetFromJsonAsync<Topic>(url);
        }


        public async Task<Question?> GetQuestionById(int questionid)
        {
            string url = $"{baseAPI}questions/{questionid}";
            return await http.GetFromJsonAsync<Question>(url);
        }

        public async void PostNewAnswer(Answer answer)
        {
            string url = $"{baseAPI}answers";
            PostAnswer postAnswer = new(answer.Username, answer.Text, answer.Question.QuestionId);
            await http.PostAsJsonAsync(url, postAnswer);
        }

        public async void PostNewQuestion(Question question)
        {
            string url = $"{baseAPI}questions";
            if(question != null) { 
                PostQuestion postQuestion = new(question.Username, question.Title, question.Text, question.Topic.TopicId);
                await http.PostAsJsonAsync(url, postQuestion);
            }
        }

        // Sender put request til api 
        public async void IncrementVotes(long id, AnswerOrQuestion type)
        {
            PutItemId newData = new(id);
            string url = $"{baseAPI}{type}/incrementvotes";
            await http.PutAsJsonAsync(url, newData);
        }
        public async void DecrementVotes(long id, AnswerOrQuestion type)
        {
            PutItemId newData = new(id);
            string url = $"{baseAPI}{type}/decrementvotes";
            await http.PutAsJsonAsync(url, newData);
        }

        private record PutItemId(long AnswerId);
        private record PostAnswer(string? username, string? text, long? questionId);
        private record PostQuestion(string? username, string? title, string? text, long? topicId);
    }
    public enum AnswerOrQuestion
    {
        answers,
        questions
    }
}
