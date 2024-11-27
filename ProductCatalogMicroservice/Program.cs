using Microsoft.EntityFrameworkCore;
using ProductCatalogMicroservice.Data;
using ProductCatalogMicroservice.Model;
using ProductCatalogMicroservice.AsyncDataServices;
using ProductCatalogMicroservice.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductCatalogDbContext>(options =>
    options.UseInMemoryDatabase("ProductCatalogDb"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();
    context.Database.EnsureCreated();

    if (!context.Products.Any())
    {
        context.Products.Add(new Product
        {
            Name = "Laptop",
            Description = "A high-performance laptop.",
            Price = 999.99m,
            Stock = 10
        });

        context.Products.Add(new Product
        {
            Name = "Smartphone",
            Description = "A latest-model smartphone.",
            Price = 699.99m,
            Stock = 20
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


