using Onyx.Models;
using Onyx.Models.Domain.ProcessData;

namespace Onyx.Repositories
{
    public interface IMongoProcessDataRepository
    {
        Task<List<ProcessDataModel>> GetManyAsync(QueryParams queryParams);
        Task<ProcessDataModel> GetOneAsync(string idSerial);
        Task<ProcessDataModel> CreateAsync(NewProcessDataModel unit);
        Task<bool> UpdateAsync(ProcessDataModel unit);
        Task<bool> DeleteAsync(string idSerial);
        void Configure(string dbName, string colName);
    }
}
