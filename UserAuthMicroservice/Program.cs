using Microsoft.EntityFrameworkCore;
using UserAuthMicroservice.AsyncDataServices;
using UserAuthMicroservice.Data;
using UserAuthMicroservice.Model;
using UserAuthMicroservice.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserAuthDbContext>(options =>
    options.UseInMemoryDatabase("UserAuthDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserAuthDbContext>();
    context.Database.EnsureCreated(); 

    if (!context.Users.Any())
    {
        context.Users.Add(new User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
