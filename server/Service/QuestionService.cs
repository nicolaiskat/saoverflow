using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using server.Data;
using mylib.Model;

namespace server.Service
{
    public class QuestionService
    {
        

        private QuestionContext db { get; }
        public QuestionService(QuestionContext db)
        {
            this.db = db;
        }

        public void SeedData()
        {
            Topic topic = db.Topics.FirstOrDefault()!;
            Topic topic2 = topic;
            if (topic == null)
            {
                topic = new Topic("C#");
                db.Topics.Add(topic);
                topic2 = new Topic("JavaScript");
                db.Topics.Add(topic2);
                db.Topics.Add(new Topic("Python"));
            }

            Question question = db.Questions.FirstOrDefault()!;
            if (question == null)
            {
                question = new Question("Line","Array", "Hvad er et array?", topic);
                db.Questions.Add(question);
                db.Questions.Add(new Question("Nico", "List", "Hvad er en liste?", topic));
                db.Questions.Add(new Question("Andreas", "JS", "Hvad er JavaScript?", topic2));
            }

            Answer answer = db.Answers.FirstOrDefault()!;
            if (answer == null)
            {
                db.Answers.Add(new Answer("Nico", "Det er en liste", question));
                db.Answers.Add(new Answer("Andreas", "sjkrfkejrghker", question));
            }
            db.SaveChanges();
        }

        // Get questions(first 15, sorted by date)
        // Vi vil undgå at have topic og answer til at vise questions
        public List<Question> GetAllQuestions()
        {
            var result = db.Questions
                .OrderByDescending(question => question.CreatedAt)
                .Include(question => question.Topic)
                .Take(15)
                .ToList();
            return result;
        }

        // Get question by id (incl. list answer, answer sorted by votes)
        public Question? GetQuestionById(long id)
        {
            var result = db.Questions
                .Where(question => question.QuestionId == id)
                //.Select(question => question.Answers.OrderBy(answer => answer.Votes))
                .Include(question => question.Topic)
                .Include(question => question.Answers.OrderByDescending(answer => answer.Votes))
                .FirstOrDefault();
            if (result == null)
                return null;
            return result;
        }

        // Get all topics (sorted by name)
        public List<Topic> GetAllTopics()
        {
            var result = db.Topics
                .OrderBy(topic => topic.Name)
                .Include(topic => topic.Questions)
                .ToList();
            return result;
        }

        // Get topic by id (incl. list question(first 15, sorted by date))
        public Topic? GetTopicById(long id)
        {
            var result = db.Topics
                .Where(topic => topic.TopicId == id)
                .Include(topic => topic.Questions
                    .OrderByDescending(questions => questions.CreatedAt)
                    .Take(15))
                .FirstOrDefault();
            if (result == null)
                return null;
            return result;
        }

        // Post question : Question
        public string CreateQuestion(string? username, string? title, string? text, long topicId)
        {
            var topic = db.Topics.Where(topic => topic.TopicId == topicId).FirstOrDefault();
            if (topic == null)
                return JsonSerializer.Serialize(new { msg = "Topic not found"});

            Question q = new (username, title, text, topic);
            db.Questions.Add(q);
            db.SaveChanges();
            return JsonSerializer.Serialize(new {msg = "New question created", newQuestion = q});
        }

        // Post answer : Answer
        public string CreateAnswer(string? username, string? text, long questionId)
        {
            var question = db.Questions.Where(question => question.QuestionId == questionId).FirstOrDefault();
            if (question == null)
                return JsonSerializer.Serialize(new { msg = "Question not found" });

            Answer a = new (username, text, question);
            db.Answers.Add(a);
            db.SaveChanges();
            return JsonSerializer.Serialize(new { msg = "New answer created", newAnswer = a });
        }

        // Put question vote increment : void
        public string QVoteIncrement(long questionId)
        {
            var question = db.Questions.Where(question => question.QuestionId == questionId).FirstOrDefault();
            if (question == null)
                return JsonSerializer.Serialize(new { msg = "Question not found" });

            question.IncrementVotes();
            db.SaveChanges();
            return JsonSerializer.Serialize(new { msg = "Vote incremented"});
        }
        
        
        // Put question vote decrement : void
        public string QVoteDecrement(long questionId)
        {
            var question = db.Questions.Where(question => question.QuestionId == questionId).FirstOrDefault();
            if (question == null)
                return JsonSerializer.Serialize(new { msg = "Question not found" });

            question.DecrementVotes();
            db.SaveChanges();
            return JsonSerializer.Serialize(new { msg = "Vote decremented" });
        }
        
        // Put answer vote increment : void
        public string AVoteIncrement(long answerId)
        {
            var answer = db.Answers.Where(answer => answer.AnswerId == answerId).FirstOrDefault();
            if (answer == null)
                return JsonSerializer.Serialize(new { msg = "Answer not found" });

            answer.IncrementVotes();
            db.SaveChanges();
            return JsonSerializer.Serialize(new { msg = "Vote incremented" });
        }

        // Put answer vote decrement : void
        public string AVoteDecrement(long answerId)
        {
            var answer = db.Answers.Where(answer => answer.AnswerId == answerId).FirstOrDefault();
            if (answer == null)
                return JsonSerializer.Serialize(new { msg = "Answer not found" });

            answer.DecrementVotes();
            db.SaveChanges();
            return JsonSerializer.Serialize(new { msg = "Vote decremented" });
        }


    }
}
