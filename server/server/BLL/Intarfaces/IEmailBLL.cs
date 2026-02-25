namespace server.BLL.Intarfaces
{
    public interface IEmailBLL
    {
        Task<bool> SendEmail(string to, string subject, string body);
    }
}
