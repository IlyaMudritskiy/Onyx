using Onyx.Models;
using Onyx.Models.Domain;

namespace Onyx.Repositories
{
    public interface IProcessDataRepository
    {
        Task<List<ProcessDataModel>> GetManyAsync(QueryParams queryParams);
        Task<ProcessDataModel> GetOneAsync(string idSerial);
        Task<ProcessDataModel> CreateAsync(NewProcessDataModel unit);
        Task<ShortProcessDataModel> UpdateAsync(ProcessDataModel unit);
        Task<ShortProcessDataModel> DeleteAsync(string idSerial);
    }
}
