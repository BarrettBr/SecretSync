using SecretSync.Application.Services;
using SecretSync.Core.Projects;
using SecretSync.Infrastructure.Stores;

// Decided to go with a controller based approach for better organization as the project grows
// Based off of this article: https://medium.com/@codebob75/creating-and-consuming-apis-in-net-c-d24f9c414b96
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Used for controller
builder.Services.AddOpenApi();  // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// TODO: Don't know if this is needed for a small app, but saw a lot of it so decided to add it. Look into later if needed.
// Scoped means these live for the duration of a request
// Didn't use singleton as that would share state across requests which we don't want for a store since it may hold resources
// Didn't use transient as that would create a new instance every time it's requested which is unnecessary overhead for our services
// More information: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage#service-lifetimes
  // Or: https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddSingleton<IProjectStore, ProjectStore>(); // TODO: Change to AddScoped later when changing from in memory -> database

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

// Might need these for authorization later was looking at [Authorize] tag but didn't know yet how to set it all up
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
