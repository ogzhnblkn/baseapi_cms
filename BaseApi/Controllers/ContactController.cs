using BaseApi.Application.DTOs.Contact;
using BaseApi.Application.Features.Contact.Commands.CreateContact;
using BaseApi.Application.Features.Contact.Queries.GetAllContacts;
using BaseApi.Application.Features.Contact.Queries.GetContact;
using BaseApi.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISpamFilterService _spamFilterService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IMediator mediator, ISpamFilterService spamFilterService, ILogger<ContactController> logger)
        {
            _mediator = mediator;
            _spamFilterService = spamFilterService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            try
            {
                var query = new GetAllContactsQuery();
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts");
                return BadRequest(new { message = "Bir hata oluştu" });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ContactDto>> GetContactById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetContactQuery { Id = id });

                if (result == null)
                    return NotFound(new { message = $"Contact with ID {id} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact {ContactId}", id);
                return BadRequest(new { message = "Bir hata oluştu" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ContactDto>> CreateContact([FromBody] CreateContactDto createContactDto)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return BadRequest(new { message = "Geçersiz veri", errors = errors });
                }

                // Spam kontrolü
                if (_spamFilterService.IsSpam(createContactDto.Message, createContactDto.Email, createContactDto.NameSurname))
                {
                    _logger.LogWarning("Spam detected from {Email}: {Message}",
                        createContactDto.Email, createContactDto.Message);

                    // Spam olduğunu belli etme, normal response dön
                    return Ok(new { message = "İletişim formunuz başarıyla gönderildi." });
                }

                // IP ve User-Agent logla
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers.UserAgent.ToString();

                _logger.LogInformation("Contact form submitted from IP: {ClientIp}, UserAgent: {UserAgent}, Email: {Email}",
                    clientIp, userAgent, createContactDto.Email);

                var command = new CreateContactCommand
                {
                    PhoneNumber = createContactDto.PhoneNumber?.Trim(),
                    City = createContactDto.City?.Trim(),
                    Email = createContactDto.Email?.Trim().ToLowerInvariant(),
                    Message = createContactDto.Message?.Trim(),
                    NameSurname = createContactDto.NameSurname?.Trim(),
                    Subject = createContactDto.Subject?.Trim()
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("Contact form successfully processed for {Email}", createContactDto.Email);

                return Ok(new { message = "İletişim formunuz başarıyla gönderildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form for {Email}", createContactDto.Email);
                return StatusCode(500, new { message = "Form gönderilirken bir hata oluştu. Lütfen tekrar deneyiniz." });
            }
        }
    }
}
