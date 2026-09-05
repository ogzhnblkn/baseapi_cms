using BaseApi.Application.Common;
using BaseApi.Application.Features.Pages;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageHandler : IRequestHandler<UpdatePageCommand, UpdatePageResponse>
    {
        private readonly IPageRepository _pageRepository;

        public UpdatePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<UpdatePageResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
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
                return new UpdatePageResponse
                {
                    Success = false,
                    Message = validationMessage
                };
            }

            var page = await _pageRepository.GetByIdAsync(request.Id);
            if (page == null)
            {
                return new UpdatePageResponse
                {
                    Success = false,
                    Message = Messages.Page.NotFound
                };
            }

            request.Title = request.Title.Trim();
            request.Slug = request.Slug.Trim();

            if (await _pageRepository.SlugExistsAsync(request.Slug, request.Id))
            {
                return new UpdatePageResponse
                {
                    Success = false,
                    Message = Messages.Page.SlugExists
                };
            }

            page.Title = request.Title;
            page.Slug = request.Slug;
            page.Summary = request.Summary;
            page.Content = request.Content;
            page.FeaturedImageUrl = request.FeaturedImageUrl;
            page.Template = (PageTemplate)request.Template;
            page.Status = (PageStatus)request.Status;
            page.IsHomePage = request.IsHomePage;
            page.Visibility = (PageVisibility)request.Visibility;
            page.AllowComments = request.AllowComments;
            page.MetaTitle = request.MetaTitle;
            page.MetaDescription = request.MetaDescription;
            page.Keywords = request.Keywords;
            page.CanonicalUrl = request.CanonicalUrl;
            page.CustomCss = request.CustomCss;
            page.CustomJs = request.CustomJs;
            page.Order = request.Order;
            page.PublishedAt = request.PublishedAt;
            page.UpdatedBy = request.UpdatedBy;

            await _pageRepository.UpdateAsync(page);

            return new UpdatePageResponse
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Message = Messages.Page.Updated,
                Success = true
            };
        }
    }
}
