using UsersHandler.Business;
// using UsersHandler.Business.Abstraction;
// using UsersHandler.Controllers;
using UsersHandler.Repository;
using UsersHandler.Repository.Abstraction;
using UsersHandler.Business.Abstraction;
using GlobalUtility.Manager;
using Microsoft.OpenApi.Models;
using UsersHandler.Business.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "UsersHandlerApi", Version = "v1" });  
    c.OperationFilter<FileUploadOperationFilter>();
});
builder.Services.AddLogging(logging => logging.AddConsole());

builder.Services.AddControllers();
builder.Services.AddDbContext<UsersHandlerDbContext>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IBusiness, Business>();

builder.Services.AddKafkaAdministratorService<KafkaTopicsOutput>(builder.Configuration);
Console.WriteLine("executed AddKafkaAdministratorService");
builder.Services.AddKafkaProducerService<KafkaTopicsOutput, ProducerService>(builder.Configuration);
Console.WriteLine("executed AddKafkaProducerService");
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

