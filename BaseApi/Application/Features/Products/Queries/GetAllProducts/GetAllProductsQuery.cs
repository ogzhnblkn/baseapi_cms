using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public ProductCategory? Category { get; set; }
        public ProductStatus? Status { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsNewProduct { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchTerm { get; set; }
        public bool ActiveOnly { get; set; } = false;
    }
}