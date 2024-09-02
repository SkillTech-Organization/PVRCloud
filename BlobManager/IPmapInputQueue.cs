using System.Threading.Tasks;

namespace BlobManager;

public interface IPmapInputQueue
{
    Task SendMessageAsync(CalcRequest request);
}
