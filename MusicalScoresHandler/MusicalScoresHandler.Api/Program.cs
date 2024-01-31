using MusicalScoresHandler.Repository;
using MusicalScoresHandler.Repository.Abstraction;
using GlobalUtility.Manager;
using Microsoft.OpenApi.Models;
using MusicalScoresHandler.Repository.Repository;
using MusicalScoresHandler.Business.Abstraction;
using MusicalScoresHandler.Business.Business;
using GlobalUtility.Kafka.Abstraction.MessageHandler;
using MusicalScoresHandler.Business.Kafka;
using GlobalUtility.Kafka.Config;
using AuthorsHandler.ClientHttp.Abstraction;
using AuthorsHandler.ClientHttp;
using UsersHandler.ClientHttp;
using UsersHandler.ClientHttp.Abstraction;

var builder = WebApplication.CreateBuilder(args);

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


// string baseAddress = builder.Configuration.GetSection("MusicalScoresHandlerClientHttp:UsersAPIBaseAddress").Value ?? throw new Exception("No such BaseAddress");
// Console.WriteLine("Section value " + baseAddress);
// Uri httpClientUri = new Uri(baseAddress);

try {
	builder.Services.AddHttpClient<IUsersHandlerClientHttp, UsersHandlerClientHttp>("MusicalScoresHandler_UsersClientHttp");
} catch (Exception) {
	Console.Error.WriteLine("MusicalScoresHandler_UsersClientHttp not found");
}

// baseAddress = builder.Configuration.GetSection("MusicalScoresHandlerClientHttp:AuthorsAPIBaseAddress").Value ?? throw new Exception("No such BaseAddress");
// Console.WriteLine("Section value " + baseAddress);
// httpClientUri = new Uri(baseAddress);

try {
	builder.Services.AddHttpClient<IAuthorsHandlerClientHttp, AuthorsHandlerClientHttp>("MusicalScoresHandler_AuthorsClientHttp");
} catch (Exception) {
	Console.Error.WriteLine("MusicalScoresHandler_UsersClientHttp not found");
}
// HttpClientFactoryServiceCollectionExtensions.AddHttpClient(builder.Services);


builder.Services.AddScoped<IKafkaTopics, KafkaTopicsInput>();
builder.Services.AddScoped<IAuthorKafkaRepository, AuthorKafkaRepository>();
builder.Services.AddScoped<IUserKafkaRepository, UserKafkaRepository>();
builder.Services.AddScoped<IMessageHandler, AuthorMessageHandler>();
builder.Services.AddScoped<IMessageHandler, UserMessageHandler>();
builder.Services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

builder.Services.AddKafkaConsumerService<KafkaTopicsInput, MessageHandlerFactory>(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicalScoresHandlerApi", Version = "v1" });
	c.OperationFilter<FileUploadOperationFilter>();
});

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

