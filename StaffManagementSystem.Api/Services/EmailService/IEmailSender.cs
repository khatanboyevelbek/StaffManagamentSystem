namespace StaffManagementSystem.Api.Services.EmailService
{
    public interface IEmailSender
    {
        void SendEmail(string userEmail, string userPassword, string actionMessage);
    }
}
