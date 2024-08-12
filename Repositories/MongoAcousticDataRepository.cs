using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Onyx.DbContext;
using Onyx.Models;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Repositories
{
    public class MongoAcousticDataRepository : IMongoAcousticDataRepository
    {
        private readonly MongoDbService _mongoDBService;
        private readonly IMapper _mapper;
        private string _dbName;
        private string _colName;

        public MongoAcousticDataRepository(IMapper mapper, MongoDbService mongoDBService)
        {
            _mapper = mapper;
            _mongoDBService = mongoDBService;
        }

        public void Configure(string dbName, string colName)
        {
            _dbName = dbName;
            _colName = colName;
        }

        public async Task<List<AcousticDataModel>> GetManyAsync(QueryParams queryParams)
        {
            return await _mongoDBService.GetManyAsync<AcousticDataModel>(queryParams, _dbName, _colName);
        }

        public async Task<AcousticDataModel> GetOneAsync(string idSerial)
        {
            AcousticDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.GetOneAsync<AcousticDataModel, ObjectId>(x => x.Id, id, _dbName, _colName);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.GetOneAsync<AcousticDataModel, string>(x => x.DUT.SerialNr, idSerial, _dbName, _colName);
            }
            return result;
        }

        public async Task<AcousticDataModel> CreateAsync(NewAcousticDataModel unit)
        {
            var unitToCreate = _mapper.Map<AcousticDataModel>(unit);
            try
            {
                return await _mongoDBService.CreateAsync(unitToCreate, _dbName, _colName);
            }
            catch (MongoWriteException ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(AcousticDataModel unit)
        {
            var updateDefinition = Builders<AcousticDataModel>.Update
                .Set(x => x.DUT, unit.DUT)
                .Set(x => x.Steps, unit.Steps);

            var success = await _mongoDBService.UpdateAsync(x => x.Id == unit.Id, updateDefinition, _dbName, _colName);
            return success;
        }

        public async Task<bool> DeleteAsync(string idSerial)
        {
            AcousticDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.DeleteAsync<AcousticDataModel, ObjectId>(x => x.Id, id, _dbName, _colName);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.DeleteAsync<AcousticDataModel, string>(x => x.DUT.SerialNr, idSerial, _dbName, _colName);
            }
            return !(result == null);
        }
    }
}
