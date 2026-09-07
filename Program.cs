using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using GreenTime.Context;

var builder = WebApplication.CreateBuilder(args);

//EF
builder.Services.AddDbContext<GreenTimeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Uygulama çalışınca direkt anasayfada açılmasını sağlar
    });
}

app.UseHttpsRedirection();

app.Run();

