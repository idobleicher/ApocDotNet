using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace MongoDAL
{
    public class ConnectToMongo
    {
        #region DataMembers

        private IMongoClient _client { get; set; }
        private IMongoDatabase  _database { get; set; }

        #endregion

        #region Signleton

        private static ConnectToMongo instance = null;

        private ConnectToMongo()
        {

        }

        public static ConnectToMongo Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ConnectToMongo();
                }

                return instance;
            }
        }

        #endregion

        #region Settings

        public void SetConnectionSettings(string host, int port, string databaseName, string username, string password)
        {
            var credentials = MongoCredential.CreateMongoCRCredential(databaseName, username, password);
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, port),
                UseSsl = true,
                Credentials = new[] { credentials }
            };
            _client = new MongoClient(settings);
            _database = _client.GetDatabase(databaseName);
        }

        public void SetConnectionSettings(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        #endregion

        #region CrudMongoMethods

        public string GetCollectionsNames()
        {
            Dictionary<string, object> dictCollectionsNames = new Dictionary<string, object>();
            foreach (BsonDocument item in _database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                JObject itemValues  = JsonConvert.DeserializeObject(item.ToJson()) as JObject;
                dictCollectionsNames.Add(itemValues["name"].ToString(), _database.GetCollection<BsonDocument>(itemValues["name"].ToString()).Count(_=> true).ToString());
            }
            return dictCollectionsNames.ToJson();
        }

        public string GetDataByCollectionName(string collectionName)
        {
             return _database.GetCollection<BsonDocument>(collectionName).Find(_ => true).ToList().ToJson();
        }

        public string GetDataByCollectionNameFillteredWithAttributeName(string collectionName, Dictionary<string, List<string>> fillterNameWithValues)
        {
            List<BsonDocument> lstValues = new List<BsonDocument>();

            bool bToAdd = true;
            foreach (BsonDocument item in _database.GetCollection<BsonDocument>(collectionName).Find(_ => true).ToList())
            {
                foreach (string attributeName in fillterNameWithValues.Keys)
                {
                    if (item.Contains(attributeName) && !(fillterNameWithValues[attributeName].Contains(item[attributeName].ToString())))
                    {
                        bToAdd = false;
                    }
                }

                if (bToAdd)
                {
                    lstValues.Add(item);
                }

                bToAdd = true;
            }


            return lstValues.ToJson();
        }


        public List<string> GetRelationsObjectIdsBySourceObjectId(string collectionName, string attributeName,string attributeValue)
        {
            List<string> lstValues = new List<string>();

            foreach (BsonDocument item in _database.GetCollection<BsonDocument>(collectionName).Find(_ => true).ToList())
            {
                if (item.Contains(attributeName) && attributeValue.Contains(item[attributeName].ToString()))
                {
                    lstValues.Add(item["_id"].ToString());
                }
            }

            return lstValues;
        }



        public void CreateNode(string collectionName, Dictionary<string, object> values)
        {
            _database.GetCollection<BsonDocument>(collectionName).InsertOne(new BsonDocument(values));
        }

        public string CreateRelation(string tableName, string sourceObjectId,string destObjectId,string relationName)
        {
            string objectId = Guid.NewGuid().ToString();
            _database.GetCollection<BsonDocument>(tableName).InsertOne(new BsonDocument(new Dictionary<string,string>(){
                {"_id" ,objectId}, { "sourceObjectId" , sourceObjectId }, { "destObjectId" , destObjectId }, { "name" , relationName } }));
            return objectId;
        }

        public void UpdateNode(string collectionName,string objectId,Dictionary<string,object> values)
        {
            BsonDocument objectValues = _database.GetCollection<BsonDocument>(collectionName).Find(new BsonDocument("_id", objectId)).ToList()[0].ToBsonDocument();
            foreach (string key in values.Keys)
            {
                if (objectValues.Contains(key))
                {
                    objectValues[key] = values[key].ToString();
                }
                else
                {
                    objectValues.Add(key.ToString(), values[key].ToString());
                }
            }
            _database.GetCollection<BsonDocument>(collectionName).FindOneAndReplace(new BsonDocument("_id", objectId), objectValues);
        }

        public void DeleteNodeByObjectID(string objectId)
        {
            foreach (var collectionName in _database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
            {
                _database.GetCollection<BsonDocument>(collectionName["name"].ToString()).DeleteOne(new BsonDocument("_id", objectId));
            }
        }

        #endregion
    }
}
