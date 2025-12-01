using BaseApi.Application.Common;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Pages.Queries.GetPageById
{
    public class GetPageByIdHandler : IRequestHandler<GetPageByIdQuery, GetPageByIdResponse>
    {
        private readonly IPageRepository _pageRepository;

        public GetPageByIdHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<GetPageByIdResponse> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
        {
            var page = await _pageRepository.GetByIdAsync(request.Id);

            if (page == null)
            {
                return new GetPageByIdResponse
                {
                    Success = false,
                    Message = Messages.Page.NotFound
                };
            }

            var pageDto = new PageDetailDto
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Summary = page.Summary,
                Content = page.Content,
                FeaturedImageUrl = page.FeaturedImageUrl,
                Template = (int)page.Template,
                TemplateName = page.Template.ToString(),
                Status = (int)page.Status,
                StatusName = page.Status.ToString(),
                IsHomePage = page.IsHomePage,
                Visibility = (int)page.Visibility,
                VisibilityName = page.Visibility.ToString(),
                AllowComments = page.AllowComments,
                MetaTitle = page.MetaTitle,
                MetaDescription = page.MetaDescription,
                Keywords = page.Keywords,
                CanonicalUrl = page.CanonicalUrl,
                CustomCss = page.CustomCss,
                CustomJs = page.CustomJs,
                Order = page.Order,
                ViewCount = page.ViewCount,
                PublishedAt = page.PublishedAt,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
                CreatedBy = page.Creator?.Username ?? "Sistem",
                UpdatedBy = page.Updater?.Username
            };

            return new GetPageByIdResponse
            {
                Page = pageDto,
                Success = true,
                Message = Messages.Page.Retrieved
            };
        }
    }
}