using Confluent.Kafka;
using EventBus;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(7001);
//});



// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Service API",
        Version = "v1",
        Description = "API for creating and querying orders",
    });

    // Include XML comments if the XML doc file is present (enabled via csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add Entity Framework
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Kafka
var kafkaConfig = new ProducerConfig
{
    BootstrapServers = builder.Configuration.GetConnectionString("Kafka")
};
builder.Services.AddSingleton<IProducer<string, string>>(new ProducerBuilder<string, string>(kafkaConfig).Build());

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = builder.Configuration.GetConnectionString("Kafka"),
    GroupId = "order-service",
    AutoOffsetReset = AutoOffsetReset.Earliest
};
builder.Services.AddSingleton<IConsumer<string, string>>(new ConsumerBuilder<string, string>(consumerConfig).Build());

builder.Services.AddScoped<IEventBus, KafkaEventBus>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();