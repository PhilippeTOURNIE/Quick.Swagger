using Quick.SwaggerWidthApiVersion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddQuickSwaggerWidthApiVersion("Demo", 2);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.AddQuickUseSwagger();

app.Run();
