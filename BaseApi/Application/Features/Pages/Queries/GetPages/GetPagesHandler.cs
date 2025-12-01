using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Pages.Queries.GetPages
{
    public class GetPagesHandler : IRequestHandler<GetPagesQuery, GetPagesResponse>
    {
        private readonly IPageRepository _pageRepository;

        public GetPagesHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<GetPagesResponse> Handle(GetPagesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Page> pages;

            if (request.PublishedOnly == true)
            {
                pages = await _pageRepository.GetPublishedAsync();
            }
            else if (request.Status.HasValue)
            {
                pages = await _pageRepository.GetByStatusAsync((PageStatus)request.Status.Value);
            }
            else if (request.Template.HasValue)
            {
                pages = await _pageRepository.GetByTemplateAsync((PageTemplate)request.Template.Value);
            }
            else
            {
                pages = await _pageRepository.GetAllAsync();
            }

            var pageDtos = pages.Select(p => new PageDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Summary = p.Summary,
                FeaturedImageUrl = p.FeaturedImageUrl,
                Template = p.Template.ToString(),
                Status = p.Status.ToString(),
                IsHomePage = p.IsHomePage,
                Visibility = p.Visibility.ToString(),
                ViewCount = p.ViewCount,
                Order = p.Order,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                CreatedBy = p.Creator?.Username ?? "Sistem"
            }).ToList();

            return new GetPagesResponse
            {
                Pages = pageDtos,
                Message = Messages.Page.ListRetrieved,
                Success = true
            };
        }
    }
}