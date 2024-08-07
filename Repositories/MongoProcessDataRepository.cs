using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Onyx.Models;
using Onyx.Models.Domain;
using Onyx.Services;
using Onyx.SignalR;

namespace Onyx.Repositories
{
    public class MongoProcessDataRepository : IProcessDataRepository
    {
        private readonly MDBProcessDataService _mongoDBService;
        private readonly IMapper _mapper;
        private readonly IHubContext<DutUpdateHub> _hub;

        public MongoProcessDataRepository(IMapper mapper, MDBProcessDataService mongoDBService, IHubContext<DutUpdateHub> hubContext)
        {
            _mapper = mapper;
            _mongoDBService = mongoDBService;
            _hub = hubContext;
        }

        public async Task<List<ProcessDataModel>> GetManyAsync(QueryParams qps)
        {
            return await _mongoDBService.GetAllAsync(qps);
        }

        public async Task<ProcessDataModel> GetOneAsync(string idSerial)
        {
            ProcessDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.GetOneAsync(x => x.Id, id);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.GetOneAsync(x => x.DUT.SerialNr, idSerial);
            }
            return result;
        }

        public async Task<ProcessDataModel> CreateAsync(NewProcessDataModel unit)
        {
            var unitToCreate = _mapper.Map<ProcessDataModel>(unit);
            try
            {
                return await _mongoDBService.CreateAsync(unitToCreate);
            }
            catch (MongoWriteException ex)
            {
                return null;
            }
        }

        public async Task<ShortProcessDataModel> UpdateAsync(ProcessDataModel unit)
        {
            var updatedUnit = await _mongoDBService.UpdateAsync(unit);
            return _mapper.Map<ShortProcessDataModel>(updatedUnit);
        }

        public async Task<ShortProcessDataModel> DeleteAsync(string idSerial)
        {
            ProcessDataModel result = null;
            try
            {
                var id = ObjectId.Parse(idSerial);
                result = await _mongoDBService.DeleteAsync(x => x.Id, id);
            }
            catch (FormatException ex)
            {
                result = await _mongoDBService.DeleteAsync(x => x.DUT.SerialNr, idSerial);
            }
            return _mapper.Map<ShortProcessDataModel>(result);
        }
    }
}