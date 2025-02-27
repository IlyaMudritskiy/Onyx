﻿using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Onyx.DbContext;
using Onyx.Models;
using Onyx.Models.Domain.ProcessData;

namespace Onyx.Repositories
{
    public class MongoProcessDataRepository : IMongoProcessDataRepository
    {
        private readonly MongoDbService _mongoDBService;
        private readonly IMapper _mapper;
        private string _dbName;
        private string _colName;

        public MongoProcessDataRepository(IMapper mapper, MongoDbService mongoDBService)
        {
            _mapper = mapper;
            _mongoDBService = mongoDBService;
        }

        public void Configure(string dbName, string colName)
        {
            _dbName = dbName;
            _colName = colName;
        }

        public async Task<List<ProcessDataModel>> GetManyAsync(QueryParams queryParams)
        {
            return await _mongoDBService.GetManyAsync<ProcessDataModel>(queryParams, _dbName, _colName);
        }

        public async Task<ProcessDataModel> GetOneAsync(string idSerial)
        {
            ProcessDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.GetOneAsync<ProcessDataModel, ObjectId>(x => x.Id, id, _dbName, _colName);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.GetOneAsync< ProcessDataModel, string>(x => x.DUT.SerialNr, idSerial, _dbName, _colName);
            }
            return result;
        }

        public async Task<ProcessDataModel> CreateAsync(NewProcessDataModel unit)
        {
            var unitToCreate = _mapper.Map<ProcessDataModel>(unit);
            try
            {
                return await _mongoDBService.CreateAsync(unitToCreate, _dbName, _colName);
            }
            catch (MongoWriteException ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(ProcessDataModel unit)
        {
            var updateDefinition = Builders<ProcessDataModel>.Update
                .Set(x => x.DUT, unit.DUT)
                .Set(x => x.Steps, unit.Steps);

            var success = await _mongoDBService.UpdateAsync(x => x.Id == unit.Id, updateDefinition, _dbName, _colName);
            return success;
        }

        public async Task<bool> DeleteAsync(string idSerial)
        {
            ProcessDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.DeleteAsync<ProcessDataModel, ObjectId>(x => x.Id, id, _dbName, _colName);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.DeleteAsync<ProcessDataModel, string>(x => x.DUT.SerialNr, idSerial, _dbName, _colName);
            }
            return !(result == null);
        }
    }
}