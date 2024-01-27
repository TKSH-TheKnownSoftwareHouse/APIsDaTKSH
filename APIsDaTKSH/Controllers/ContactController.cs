using APIsDaTKSH.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace APIsDaTKSH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactController> _logger;
        private readonly MyDbContext _dbContext;

        public ContactController(IConfiguration configuration, ILogger<ContactController> logger, MyDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactModel contactForm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Adiciona as informações ao banco de dados
                var contactEntity = new ContactModel
                {
                    id = contactForm.id,
                    FullName = contactForm.FullName,
                    Email = contactForm.Email,
                    Message = contactForm.Message
                };

                _dbContext.Contacts.Add(contactEntity);
                await _dbContext.SaveChangesAsync();

                await SendEmail(contactForm);

                return Ok("Formulário de contato recebido com sucesso!");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail");
                return StatusCode(500, "Erro ao enviar e-mail.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro interno: {ex.Message}");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        [HttpGet]
        public IActionResult GetAllContacts()
        {
            var contacts = _dbContext.Contacts.ToList();
            if (contacts == null)
            {
                return NotFound($"Contacts not found.");
            }

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _dbContext.Contacts.FirstOrDefault(c => c.id == id);

            if (contact == null)
            {
                return NotFound($"Contact with ID {id} not found.");
            }

            return Ok(contact);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactModel updatedContact)
        {
            var existingContact = _dbContext.Contacts.FirstOrDefault(c => c.id == id);

            if (existingContact == null)
            {
                return NotFound($"Contact with ID {id} not found.");
            }

            existingContact.FullName = updatedContact.FullName;
            existingContact.Email = updatedContact.Email;
            existingContact.Message = updatedContact.Message;

            await _dbContext.SaveChangesAsync();

            return Ok($"Contact with ID {id} updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contactToDelete = _dbContext.Contacts.FirstOrDefault(c => c.id == id);

            if (contactToDelete == null)
            {
                return NotFound($"Contact with ID {id} not found.");
            }

            _dbContext.Contacts.Remove(contactToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok($"Contact with ID {id} deleted successfully.");
        }
        private async Task SendEmail(ContactModel contactForm)
        {
            using (var mailMessage = new MailMessage())
            using (var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"]))
            {
                mailMessage.From = new MailAddress(_configuration["EmailSettings:SenderEmail"]);
                mailMessage.Subject = "New contact form";
                var recipientsEmails = _configuration.GetSection("EmailSettings:ReceiverEmails").Get<List<string>>();

                foreach (var receiverEmail in recipientsEmails)
                {
                    mailMessage.To.Add(receiverEmail);
                }
                mailMessage.Body = $"FullName: {contactForm.FullName}\nEmail: {contactForm.Email}\nMessage: {contactForm.Message}";

                smtpClient.Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                );
                smtpClient.EnableSsl = true;
                smtpClient.Port = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                _logger.LogInformation($"SMTP Server: {_configuration["EmailSettings:SmtpServer"]}");
                _logger.LogInformation($"Sender Email: {_configuration["EmailSettings:SenderEmail"]}");
                _logger.LogInformation($"Receiver Emails: {string.Join(", ", recipientsEmails)}");
                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);



            }
        }
    }
}