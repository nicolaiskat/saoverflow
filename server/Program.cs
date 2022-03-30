using server.Service;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using server.Data;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions()
    {
        WebRootPath = "wwwroot"
    }
);

// Swagger-halløj der tilføjer nogle udviklingsværktøjer direkte i app'en.
// Se mere her: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS skal slåes til i app'en. Ellers kan man ikke hente data fra den
// fra et andet domæne.
// Se mere her: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"Environment Name: {builder.Environment.EnvironmentName}");
Console.WriteLine($"ContentRoot Path: {builder.Environment.ContentRootPath}");
Console.WriteLine($"WebRootPath: {builder.Environment.WebRootPath}");

// Tilføj DbContext factory som service.
// Det gør at man kan få TodoContext ind via dependecy injection - fx 
// i DataService (smart!)
builder.Services.AddDbContext<QuestionContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuestionContextSQLite")));

// Kan vise flotte fejlbeskeder i browseren hvis der kommer fejl fra databasen
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<QuestionService>();

// Her kan man styrer hvordan den laver JSON.
builder.Services.Configure<JsonOptions>(options =>
{
    // Super vigtig option! Den gør, at programmet ikke smider fejl
    // når man returnerer JSON med objekter, der refererer til hinanden.
    // (altså dobbelrettede associeringer)
    options.SerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Byg app'ens objekt
var app = builder.Build();

// Sørg for at HTML mv. også kan serveres
var options = new DefaultFilesOptions();
options.DefaultFileNames.Clear();
options.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(options);
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true
});

// Seed data hvis nødvendigt
using (var scope = app.Services.CreateScope())
{
    // Med scope kan man hente en service.
    var dataService = scope.ServiceProvider.GetRequiredService<QuestionService>();
    dataService.SeedData(); // Fylder data på hvis databasen er tom.
}

// Sæt Swagger og alt det andet halløj op
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

// Middlware der kører før hver request. Alle svar skal have ContentType: JSON.
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// Herunder alle endpoints i API'en
app.MapGet("/api/questions", (QuestionService service) =>
{
    return service.GetAllQuestions();
});

app.MapGet("/api/questions/{id}", (QuestionService service, long id) =>
{
    return service.GetQuestionById(id);
});


app.MapGet("/api/topics", (QuestionService service) =>
{
    return service.GetAllTopics();
});

app.MapGet("/api/topics/{id}", (QuestionService service, long id) =>
{
    return service.GetTopicById(id);
});



app.MapPost("/api/questions", (QuestionService service, QuestionApi data) =>
{
    return service.CreateQuestion(data.username, data.title, data.text, data.topicId);
});

app.MapPost("/api/answers", (QuestionService service, AnswerApi data) =>
{
    return service.CreateAnswer(data.username, data.text, data.questionId);
});


app.MapPut("/api/questions/incrementvotes", (QuestionService service, QuestionDataId questionId) =>
{
    return service.QVoteIncrement(questionId.questionId);
});

app.MapPut("/api/questions/decrementvotes", (QuestionService service, QuestionDataId questionId) =>
{
    return service.QVoteDecrement(questionId.questionId);
});


app.MapPut("/api/answers/incrementvotes", (QuestionService service, AnswerDataId answerId) =>
{
    return service.AVoteIncrement(answerId.answerId);
});

app.MapPut("/api/answers/decrementvotes", (QuestionService service, AnswerDataId answerId) =>
{
    return service.AVoteDecrement(answerId.answerId);
});


app.Run();


record QuestionApi(string? username, string? title, string? text, long topicId);
record AnswerApi(string? username, string? text, long questionId);
record QuestionDataId(long questionId);
record AnswerDataId(long answerId);
