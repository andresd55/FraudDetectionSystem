using Microsoft.EntityFrameworkCore;
using AntiFraudService.Application.Services;
using AntiFraudService.Infrastructure.Persistence;
using AntiFraudService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<FraudDetectionService>();
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
