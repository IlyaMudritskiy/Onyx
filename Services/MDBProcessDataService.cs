using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Onyx.Models;
using Onyx.Models.Domain;
using System.Linq.Expressions;

namespace Onyx.Services
{
    public class MDBProcessDataService
    {
        private readonly IMongoCollection<ProcessDataModel> _collection;

        public MDBProcessDataService(IOptions<MongoDBSettings> mongoSettings)
        {
            MongoClient client = new MongoClient(mongoSettings.Value.ConnectionURI);
            IMongoDatabase db = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _collection = db.GetCollection<ProcessDataModel>(mongoSettings.Value.CollectionName);
        }

        public async Task<List<ProcessDataModel>> GetAllAsync(QueryParams qps)
        {
            var filter = _buildFilterFromQuery(qps.FilterField, qps.FilterValue);
            var sorting = _buildSortingFromQuery(qps.SortBy, qps.IsAscending);

            var res = _collection
                .Find(filter)
                .Sort(sorting)
                .Skip((qps.Page - 1) * qps.PageSize)
                .Limit(qps.PageSize);
            
            return await res.ToListAsync();
        }

        public async Task<ProcessDataModel> CreateAsync(ProcessDataModel newUnit)
        {
            await _collection.InsertOneAsync(newUnit);
            return newUnit;
        }

        public async Task<ProcessDataModel> GetOneAsync<T>(Expression<Func<ProcessDataModel, T>> criteria, T field)
        {
            var filter = Builders<ProcessDataModel>.Filter.Eq(criteria, field);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<ProcessDataModel> UpdateAsync(ProcessDataModel newUnit)
        {
            var oldUnit = await GetOneAsync(x => x.DUT.SerialNr, newUnit.DUT.SerialNr);

            if (oldUnit == null)
                return null;

            var updated = new ProcessDataModel
            {
                Id = oldUnit.Id,
                DUT = newUnit.DUT,
                Steps = newUnit.Steps
            };

            var filter = Builders<ProcessDataModel>.Filter.Eq(x => x.DUT.SerialNr, newUnit.DUT.SerialNr);
            await _collection.ReplaceOneAsync(filter, updated);

            return updated;
        }

        public async Task<ProcessDataModel> DeleteAsync<T>(Expression<Func<ProcessDataModel, T>> criteria, T field)
        {
            var filter = Builders<ProcessDataModel>.Filter.Eq(criteria, field);
            return await _collection.FindOneAndDeleteAsync(filter);
        }

        private FilterDefinition<ProcessDataModel> _buildFilterFromQuery(List<string> filterField = null, List<string> filterValue = null)
        {
            FilterDefinition<ProcessDataModel> filter = Builders<ProcessDataModel>.Filter.Empty;

            if ((filterField == null || filterField.Count == 0) && (filterValue == null || filterValue.Count == 0))
                return filter;

            for (int i = 0; i < filterField.Count; i++)
            {
                filter &= Builders<ProcessDataModel>.Filter.Eq(filterField[i], filterValue[i]);
            }

            return filter;
        }

        private SortDefinition<ProcessDataModel> _buildSortingFromQuery(string sortBy = null, bool isAscending = true)
        {
            SortDefinition<ProcessDataModel> sortDefinition;

            if (sortBy == null)
                return Builders<ProcessDataModel>.Sort.Ascending("DUT.Serial");

            sortDefinition = isAscending
                ? Builders<ProcessDataModel>.Sort.Ascending(sortBy)
                : Builders<ProcessDataModel>.Sort.Descending(sortBy);

            return sortDefinition;
        }
    }
}
