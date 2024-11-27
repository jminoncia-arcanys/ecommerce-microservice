using Microsoft.AspNetCore.Mvc;
using ProductCatalogMicroservice.Dto;
using ProductCatalogMicroservice.Model;
using ProductCatalogMicroservice.AsyncDataServices;
using ProductCatalogMicroservice.Repositories;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly HttpClient _httpClient;
    private readonly string _userAuthServiceUrl = "http://userauth-service:8080/api/User/";
    private readonly IRabbitMQPublisher _rabbitMQPublisher;

    public ProductController(IProductRepository productRepository, ICartItemRepository cartItemRepository, HttpClient httpClient, IRabbitMQPublisher rabbitMQPublisher)
    {
        _productRepository = productRepository;
        _cartItemRepository = cartItemRepository;
        _httpClient = httpClient;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound("Product not found.");
        }

        return Ok(product);
    }

    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart([FromBody] CartItem cartItem)
    {
        if (cartItem == null || cartItem.ProductId <= 0 || cartItem.Quantity <= 0 || cartItem.UserId <= 0)
        {
            return BadRequest("Invalid cart item. ProductId, Quantity, and UserId are required.");
        }

        var userAuthResponse = await _httpClient.GetAsync($"{_userAuthServiceUrl}validate/{cartItem.UserId}");
        if (!userAuthResponse.IsSuccessStatusCode)
        {
            return Unauthorized("User not authenticated.");
        }

        var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
        if (product == null)
        {
            return NotFound("Product not found.");
        }

        var updatedCartItem = await _cartItemRepository.AddOrUpdateCartItemAsync(cartItem);

        var eventData = new
        {
            Action = "AddToCart",
            UserId = cartItem.UserId,
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity
        };

        // Send event to RabbitMQ (Publish event)
        await _rabbitMQPublisher.PublishAsync(eventData);


        return Ok(new { message = "Product added to cart successfully.", cartItem = updatedCartItem });
    }


    [HttpPost("manage-inventory")]
    public async Task<IActionResult> ManageInventory([FromBody] Product product)
    {
        if (product == null || product.ProductId <= 0)
        {
            return BadRequest("Invalid product.");
        }

        var existingProduct = await _productRepository.GetProductByIdAsync(product.ProductId);

        if (existingProduct != null)
        {
            existingProduct.Stock += product.Stock;
            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            return Ok(new { message = "Product inventory updated successfully.", existingProduct = updatedProduct });
        }

        var newProduct = await _productRepository.AddProductAsync(product);
        return Ok(new { message = "Product added to inventory successfully.", product = newProduct });
    }
}
