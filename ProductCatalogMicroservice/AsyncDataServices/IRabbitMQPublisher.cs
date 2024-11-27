namespace ProductCatalogMicroservice.AsyncDataServices
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync(object eventData);
    }
}
