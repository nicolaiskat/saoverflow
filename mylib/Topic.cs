namespace mylib.Model
{
    public class Topic
    {
        public Topic()
        {
        }

        public Topic(string name)
        {
            Name = name;
        }

        public long TopicId { get; set; }

        public string Name { get; set; } = "NotNamed";

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
