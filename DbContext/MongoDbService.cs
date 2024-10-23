using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Onyx.Models;
using System.Linq.Expressions;

namespace Onyx.DbContext
{
    /// <summary>
    /// Provides services for interacting with MongoDB databases, including CRUD operations and query building.
    /// </summary>
    public class MongoDbService
    {
        private readonly MongoClient _client;

        public MongoDbService(IOptions<MongoDBSettings> mongoSettings)
        {
            _client = new MongoClient(mongoSettings.Value.ConnectionURI);
        }

        /// <summary>
        /// Retrieves a collection from the specified database and collection names.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The requested collection.</returns>
        private IMongoCollection<TModel> _getCollection<TModel>(string databaseName, string collectionName)
        {
            var db = _client.GetDatabase(databaseName);
            return db.GetCollection<TModel>(collectionName);
        }

        /// <summary>
        /// Retrieves multiple documents based on query parameters.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <param name="qps">The query parameters for filtering, sorting, and pagination.</param>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>A list of documents matching the query parameters.</returns>
        public async Task<List<TModel>> GetManyAsync<TModel>(QueryParams qps, string databaseName, string collectionName)
        {
            var collection = _getCollection<TModel>(databaseName, collectionName);
            var filter = _buildFilterFromQuery<TModel>(qps);
            var sorting = _buildSortingFromQuery<TModel>(qps.SortBy, qps.IsAscending);

            var res = collection
                .Find(filter)
                .Sort(sorting)
                .Skip((qps.Page - 1) * qps.PageSize)
                .Limit(qps.PageSize);

            return await res.ToListAsync();
        }

        /// <summary>
        /// Retrieves a single document based on a specified criterion.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <typeparam name="TField">The type of the field used for filtering.</typeparam>
        /// <param name="criteria">An expression defining the criterion for filtering documents.</param>
        /// <param name="fieldValue">The value to match against the specified criterion.</param>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The first document matching the criterion; null if no match is found.</returns>
        public async Task<TModel> GetOneAsync<TModel, TField>(Expression<Func<TModel, TField>> criteria, TField fieldValue, string databaseName, string collectionName)
        {
            var collection = _getCollection<TModel>(databaseName, collectionName);
            var filter = Builders<TModel>.Filter.Eq(criteria, fieldValue);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Inserts a new document into the specified collection.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <param name="newUnit">The document to insert.</param>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The inserted document.</returns>
        public async Task<TModel> CreateAsync<TModel>(TModel newUnit, string databaseName, string collectionName)
        {
            var collection = _getCollection<TModel>(databaseName, collectionName);
            await collection.InsertOneAsync(newUnit);

            return newUnit;
        }

        /// <summary>
        /// Updates a document in the specified collection based on a filter expression and update definition.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <param name="filterExpression">An expression defining the filter criteria for selecting the document(s) to update.</param>
        /// <param name="updateDefinition">The update operations to apply to the selected document(s).</param>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>True if at least one document was matched and modified; otherwise, false.</returns>
        public async Task<bool> UpdateAsync<TModel>(
            Expression<Func<TModel, bool>> filterExpression,
            UpdateDefinition<TModel> updateDefinition,
            string databaseName, string collectionName)
        {
            var collection = _getCollection<TModel>(databaseName, collectionName);
            var result = await collection.UpdateOneAsync(filterExpression, updateDefinition);

            return result.MatchedCount > 0 && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Deletes a document from the specified collection based on a criterion.
        /// </summary>
        /// <typeparam name="TModel">The model representing the documents in the collection.</typeparam>
        /// <typeparam name="TField">The type of the field used for filtering.</typeparam>
        /// <param name="criteria">An expression defining the criterion for selecting the document to delete.</param>
        /// <param name="fieldValue">The value to match against the specified criterion.</param>
        /// <param name="databaseName">The name of the database containing the collection.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>The deleted document; null if no document matches the criterion.</returns>
        public async Task<TModel> DeleteAsync<TModel, TField>(Expression<Func<TModel, TField>> criteria, TField field, string databaseName, string collectionName)
        {
            var collection = _getCollection<TModel>(databaseName, collectionName);
            var filter = Builders<TModel>.Filter.Eq(criteria, field);
            return await collection.FindOneAndDeleteAsync(filter);
        }

        /// <summary>
        /// Builds a mongo filter definition from query parameters for filtering documents.
        /// </summary>
        /// <typeparam name="T">The model representing the documents in the collection.</typeparam>
        /// <param name="filterField">A list of field names to filter documents by.</param>
        /// <param name="filterValue">A list of values corresponding to the fields specified in filterField.</param>
        /// <returns>A filter definition constructed from the query parameters.</returns>
        private FilterDefinition<T> _buildFilterFromQuery<T>(QueryParams qps)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;

            if (qps.FilterField != null)
            {
                for (int i = 0; i < qps.FilterField.Count; i++)
                {
                    filter &= Builders<T>.Filter.Eq(qps.FilterField[i], qps.FilterValue[i]);
                }
            }
            
            if (qps.FromDate.HasValue && qps.ToDate.HasValue)
            {
                if (qps.FromDate.HasValue)
                    filter &= Builders<T>.Filter.Gte("DUT.CreatedAt", qps.FromDate.Value);

                if (qps.ToDate.HasValue)
                    filter &= Builders<T>.Filter.Lte("DUT.CreatedAt", qps.ToDate.Value);
            }

            return filter;
        }

        /// <summary>
        /// Builds a mongo sort definition from query parameters for sorting documents.
        /// </summary>
        /// <typeparam name="T">The model representing the documents in the collection.</typeparam>
        /// <param name="sortBy">The field name to sort documents by.</param>
        /// <param name="isAscending">True to sort in ascending order; false to sort in descending order.</param>
        /// <returns>A sort definition constructed from the query parameters.</returns>
        private SortDefinition<T> _buildSortingFromQuery<T>(string sortBy = null, bool isAscending = true)
        {
            SortDefinition<T> sortDefinition;

            if (sortBy == null)
                return Builders<T>.Sort.Ascending("DUT.Serial");

            sortDefinition = isAscending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return sortDefinition;
        }
    }
}
