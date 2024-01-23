using AuthorsHandler.Business;
using AuthorsHandler.Business.Abstraction;
using AuthorsHandler.Business.Kafka;
using AuthorsHandler.Repository;
using AuthorsHandler.Repository.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDbContext<AuthorsHandlerDbContext>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IBusiness, Business>();

builder.Services.AddKafkaAdministratorService<KafkaTopicsOutput>(builder.Configuration);
Console.WriteLine("executed AddKafkaAdministratorService");
builder.Services.AddKafkaProducerService<KafkaTopicsOutput, ProducerService>(builder.Configuration);
Console.WriteLine("executed AddKafkaProducerService");

// object value = builder.Services.AddAutoMapper(typeof(AssemblyMarker));

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

