﻿namespace mylib.Model
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
        public string? Username { get; set; }

        public string? Title { get; set; }

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
