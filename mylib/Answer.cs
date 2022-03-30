namespace mylib.Model
{
    public class Answer
    {
        public Answer()
        {
        }

        public Answer(string? username, string? text, Question question)
        {
            Username = username;
            Text = text;
            Question = question;
        }

        public long AnswerId { get; set; }
        public string? Username { get; set; }
        public string? Text { get; set; }
        public int Votes { get; set; } = 0;

        public Question Question { get; set; }

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
