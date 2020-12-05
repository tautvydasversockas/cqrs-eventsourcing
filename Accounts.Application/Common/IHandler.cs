using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.Common
{
    public interface IHandler<in TMessage>
    {
        Task HandleAsync(TMessage message, CancellationToken token = default);
    }
}