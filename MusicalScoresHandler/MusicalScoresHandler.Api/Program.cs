using MusicalScoresHandler.Repository;
using MusicalScoresHandler.Repository.Abstraction;
using GlobalUtility.Manager;
using Microsoft.OpenApi.Models;
using MusicalScoresHandler.Repository.Repository;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Business.Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicalScoresHandlerApi", Version = "v1" });  
    c.OperationFilter<FileUploadOperationFilter>();
});
builder.Services.AddLogging(logging => logging.AddConsole());

builder.Services.AddControllers();
builder.Services.AddDbContext<MusicalScoresHandlerDbContext>();
builder.Services.AddScoped<ICopyrightRepository, CopyrightRepository>();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IScoreGenreRelationshipRepository, ScoreGenreRelationshipRepository>();
builder.Services.AddScoped<IPdfFilesRepository, PdfFilesRepository>();
builder.Services.AddScoped<IMusicalScoresRepository, MusicalScoresRepository>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IBusiness, Business>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
// }
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Starting the app");
app.Run();

