using Service.Models.Email;

namespace Service.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="emailDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendAsync(EmailDto emailDto, CancellationToken cancellationToken = default);
}
