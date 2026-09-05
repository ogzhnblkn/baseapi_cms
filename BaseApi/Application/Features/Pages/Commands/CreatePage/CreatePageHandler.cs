using BaseApi.Application.Common;
using BaseApi.Application.Features.Pages;
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
            var validationMessage = PageCommandValidation.Validate(
                request.Title,
                request.Slug,
                request.Content,
                request.Summary,
                request.FeaturedImageUrl,
                request.Template,
                request.Status,
                request.Visibility,
                request.MetaTitle,
                request.MetaDescription,
                request.Keywords,
                request.CanonicalUrl);

            if (validationMessage != null)
            {
                return new CreatePageResponse
                {
                    Success = false,
                    Message = validationMessage
                };
            }

            request.Title = request.Title.Trim();
            request.Slug = request.Slug.Trim();

            if (await _pageRepository.SlugExistsAsync(request.Slug))
            {
                return new CreatePageResponse
                {
                    Success = false,
                    Message = Messages.Page.SlugExists
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
