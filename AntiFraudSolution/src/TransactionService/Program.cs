using Microsoft.EntityFrameworkCore;

using TransactionService.Application.Interfaces;
using TransactionService.Application.Services;
using TransactionService.Infrastructure.Persistence;
using TransactionService.Infrastructure.ExternalServices;
using TransactionService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient<AntiFraudHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5188/");
});

builder.Services.AddScoped<ITransactionService, TransactionServiceImpl>();
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
