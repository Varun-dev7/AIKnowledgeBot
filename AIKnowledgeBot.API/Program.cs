using AIKnowledgeBot.API.Extensions;
using AIKnowledgeBot.InterFace.IAI;
using AIKnowledgeBot.Models.Common;
using AIKnowledgeBot.Services.AI;

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

builder.Services.AddHttpClient<IGeminiClient, GeminiClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
