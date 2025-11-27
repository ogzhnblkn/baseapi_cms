using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Products.Queries.GetProductsByCategory
{
    public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
    {
        public ProductCategory Category { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }
}