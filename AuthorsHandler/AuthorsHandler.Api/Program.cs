using AuthorsHandler.Business;
using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Controllers;
using AuthorsHandler.Repository;
using AuthorsHandler.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(logging => logging.AddConsole());

builder.Services.AddControllers();
builder.Services.AddDbContext<AuthorsHandlerDbContext>();
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

