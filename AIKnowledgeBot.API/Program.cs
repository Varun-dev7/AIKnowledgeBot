using AIKnowledgeBot.API.Extensions;
using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.InterFace.IService;
using AIKnowledgeBot.Models.Common;
using AIKnowledgeBot.Services.AI;
using AIKnowledgeBot.Services.Background;
using AIKnowledgeBot.Services.QueryRewrite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini"));
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

builder.Services.AddHostedService<DocumentProcessingWorker>();
builder.Services.AddScoped<IQueryRewriteService, QueryRewriteService>();
builder.Services.AddHttpClient<IGeminiClient, GeminiClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    Status = "Running",
    Message = "AI Knowledge Bot API",
    Time = DateTime.UtcNow
}));

app.Run();
