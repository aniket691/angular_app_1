using MoniteringSystem.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ConfigureLogging();

// Configure services
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add global filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
    options.Filters.Add<AuthLoggingFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
