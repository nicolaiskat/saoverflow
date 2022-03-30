using Microsoft.EntityFrameworkCore;
using mylib.Model;


namespace server.Data
{
    public class QuestionContext : DbContext
    {
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<Topic> Topics => Set<Topic>();

        public QuestionContext(DbContextOptions<QuestionContext> options) : base(options) { }


    }
}
