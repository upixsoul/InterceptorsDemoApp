using InterceptorDemoApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registration for all interception-related services
builder.Services.AddInterceptors();

builder.Services.AddEndpointsApiExplorer();

// Registration for Swagger/OpenAPI documentation generation
builder.Services.AddScalarWithSawgger();

var app = builder.Build();
// Enable Swagger and Scalar UI in Development
app.UseSwaggerForDevelopment();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
