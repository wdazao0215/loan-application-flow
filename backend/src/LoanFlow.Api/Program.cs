using System.Text.Json.Serialization;
using LoanFlow.Api.ErrorHandling;
using LoanFlow.Application;
using LoanFlow.Infrastructure;
using LoanFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ClientErrorExceptionHandler>();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapControllers();

await app.Services.MigrateDatabaseAsync();

await app.RunAsync();
