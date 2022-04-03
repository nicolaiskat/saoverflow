using System.ComponentModel.DataAnnotations;

namespace mylib.Model
{
    public class Question
    {
        public Question()
        {
        }

        public Question(string? username, string? title, string? text, Topic topic)
        {
            Username = username;
            Title = title;
            Text = text;
            Topic = topic;
        }

        public long QuestionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required, StringLength(16, ErrorMessage = "Username too long (16 character limit).")]
        public string? Username { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Text { get; set; }

        public int Votes { get; set; } = 0;


        public List<Answer> Answers { get; set; } = new List<Answer>();
        public Topic Topic { get; set; }


       public void IncrementVotes()
        {
            Votes++;
        }

        public void DecrementVotes()
        {
            Votes--;
        }
    }
}
