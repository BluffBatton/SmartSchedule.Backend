using SmartSchedule.API.Middleware;
using SmartSchedule.Application;
using SmartSchedule.Application.Common.Configuration;
using SmartSchedule.Infrastructure.Persistance;
using SmartSchedule.Infrastructure.Persistance.Seeding;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "SmartScheduleCors";

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddPersistence(builder.Configuration)
    .AddAuth(builder.Configuration)
    .AddBackgroundJobs(builder.Configuration)
    .AddApplicationLayer();

builder.Services.Configure<BookingRulesOptions>(
    builder.Configuration.GetSection(BookingRulesOptions.SectionName));

var frontendOrigin = builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

await app.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
