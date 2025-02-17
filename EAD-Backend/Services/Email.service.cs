using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //! Send email using MailKit and Gmail SMTP
    public async Task<bool> SendEmailAsync(EmailDTO dto)
    {
        string email = dto.Email;
        string subject = dto.Subject;
        string message = dto.Message;

        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SuperMart Team", _configuration["SMTP:User"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            string fullMessage = message + @"<br><br><p>Regards,<br>SuperMart Team</p>";

            // Create email body with HTML content
            emailMessage.Body = new TextPart("html")
            {
                Text = fullMessage
            };

            using (var smtpClient = new SmtpClient())
            {
                // Connect to Gmail SMTP server
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_configuration["SMTP:User"], _configuration["SMTP:Password"]);

                // Send the email
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error sending email: {ex.Message},  {ex}");
            return false;
        }
    }
}
