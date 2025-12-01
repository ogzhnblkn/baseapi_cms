using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.CreatePage
{
    public class CreatePageHandler : IRequestHandler<CreatePageCommand, CreatePageResponse>
    {
        private readonly IPageRepository _pageRepository;

        public CreatePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<CreatePageResponse> Handle(CreatePageCommand request, CancellationToken cancellationToken)
        {
            // Slug kontrolü
            if (await _pageRepository.SlugExistsAsync(request.Slug))
            {
                return new CreatePageResponse
                {
                    Success = false,
                    Message = "Bu slug zaten kullanýmda!"
                };
            }

            var page = new Page
            {
                Title = request.Title,
                Slug = request.Slug,
                Summary = request.Summary,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                Template = (PageTemplate)request.Template,
                Status = (PageStatus)request.Status,
                IsHomePage = request.IsHomePage,
                Visibility = (PageVisibility)request.Visibility,
                AllowComments = request.AllowComments,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                Keywords = request.Keywords,
                CanonicalUrl = request.CanonicalUrl,
                CustomCss = request.CustomCss,
                CustomJs = request.CustomJs,
                Order = request.Order,
                PublishedAt = request.PublishedAt,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdPage = await _pageRepository.CreateAsync(page);

            return new CreatePageResponse
            {
                Id = createdPage.Id,
                Title = createdPage.Title,
                Slug = createdPage.Slug,
                Message = Messages.Page.Created,
                Success = true
            };
        }
    }
}