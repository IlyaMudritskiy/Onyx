using Onyx.Models;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Repositories
{
    public interface IMongoAcousticDataRepository
    {
        Task<List<AcousticDataModel>> GetManyAsync(QueryParams queryParams);
        Task<AcousticDataModel> GetOneAsync(string idSerial);
        Task<AcousticDataModel> CreateAsync(NewAcousticDataModel unit);
        Task<bool> UpdateAsync(AcousticDataModel unit);
        Task<bool> DeleteAsync(string idSerial);
        void Configure(string dbName, string colName);
    }
}
