using System;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LadeviVentasApi.Models
{
    public static class EmailSenderExtensions
    {
        public static event EventHandler OnEmailEvents = (sender, args) => { };
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link, string code)
        {
            OnEmailEvents.Invoke(code, EventArgs.Empty);
            return emailSender.SendEmailAsync(email, "Confirma tu email",
                $"Confirme su cuenta haciendo clic en este enlace: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
        public static Task SendEmailResetPasswordAsync(this IEmailSender emailSender, string email, string link, string token)
        {
            OnEmailEvents.Invoke(token, EventArgs.Empty);
            return emailSender.SendEmailAsync(email, "Recupera tu password",
                $"Restablezca su cuenta haciendo clic en este enlace: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}