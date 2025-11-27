using BaseApi.Application.DTOs.Product;
using MediatR;

namespace BaseApi.Application.Features.Products.Queries.GetProduct
{
    public class GetProductQuery : IRequest<ProductDto?>
    {
        public int? Id { get; set; }
        public string? Slug { get; set; }
        public bool IncrementViewCount { get; set; } = false;
    }
}