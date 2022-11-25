using NightscoutClientDotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NightscoutClientDotNet.Interfaces
{
    public interface INightscoutRestApiClient
    {
        Task<IEnumerable<Entry>> GetEntriesAsync(string query, int count);
    }
}
