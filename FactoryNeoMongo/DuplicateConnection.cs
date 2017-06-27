using MongoDAL;
using NeoDAL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace FactoryNeoMongo
{
    public class DuplicateConnection : IEnumerable
    {
        #region DataMembers

        public object GetCollectionsNames { get { return JsonConvert.DeserializeObject(ConnectToMongo.Instance.GetCollectionsNames()); } set { } }
        public object GetLabelsNames { get { return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetLabelsNames()); } set { } }

        #endregion

        #region Singleton

        private static DuplicateConnection instance = null;

        private DuplicateConnection()
        {
            ConnectToMongoNeo();
        }

        public static DuplicateConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DuplicateConnection();
                }

                return instance;
            }
        }

        #endregion

        #region Settings/Connection

        private void ConnectToMongoNeo()
        {
            if (ConfigurationManager.ConnectionStrings["MongoConnectionString"] != null && ConfigurationManager.ConnectionStrings["NeoConnectionString"] != null)
            {
                ConnectToMongo.Instance.SetConnectionSettings(ConfigurationManager.ConnectionStrings["MongoConnectionString"].ConnectionString,
                                                              ConfigurationManager.ConnectionStrings["MongoDataBaseName"].ConnectionString);
                ConnectToNeo.Instance.SetConnectionSettings(ConfigurationManager.ConnectionStrings["NeoConnectionString"].ConnectionString,
                                                            ConfigurationManager.ConnectionStrings["NeoUserName"].ConnectionString,
                                                            ConfigurationManager.ConnectionStrings["NeoPassword"].ConnectionString);
            }
            else
            {
                ConnectSettings();
            }

        }

        public void ConnectSettings()
        {
            ConnectToMongo.Instance.SetConnectionSettings("mongodb://localhost:27017/","test");
            ConnectToNeo.Instance.SetConnectionSettings("Bolt://localhost:7687", "neo4j", "nv1vnmc2");
        }

        #endregion

        #region IsValid

        private void IsValid()
        {
             new Exception("Canno't Make it That Way");
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CrudMethods

        public object GetNeoDataByQuery(string query)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetDataByQuery(query));
        }

        public object GetDataByCollectionName(string collectionName)
        {
            return JsonConvert.DeserializeObject(ConnectToMongo.Instance.GetDataByCollectionName(collectionName));
        }

        public object GetDataByLabelName(string labelName)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetDataByLabelName(labelName));
        }

        public object GetDataByCollectionNameFillteredWithAttributeName(string collectionName, Dictionary<string, List<string>> fillterNameWithValues)
        {
            return JsonConvert.DeserializeObject(ConnectToMongo.Instance.GetDataByCollectionNameFillteredWithAttributeName(collectionName, fillterNameWithValues));
        }

        public object GetDataByLabelNameFillteredWithAttributeName(string labelName, Dictionary<string,List<string>> fillterNameWithValues)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetDataByLabelNameFillteredWithAttributeName(labelName, fillterNameWithValues));
        }

        public object GetRelationsLabelsByObjectId(string labelName, string objectId)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetRelationsByObjectId(labelName,objectId));
        }

        public object GetAllRelationsCollectionsBySourceObjectId(string collectionName, string sourceObjectId)
        {
            return JsonConvert.DeserializeObject(ConnectToMongo.Instance.GetDataByCollectionNameFillteredWithAttributeName(collectionName, new Dictionary<string, List<string>>() { { "sourceObjectId", new List<string>() { sourceObjectId } } }));
        }

        public object GetRelationsByObjectIdAndFillterRelationsWithTableName(string labelName, string objectId, string relatedTableName)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetRelationsByObjectIdAndFillterRelationsWithTableName(labelName, objectId, relatedTableName));
        }

        public object GetRelationsByObjectIdAndFillterRelationsWithParameter(string labelName,string objectId, string attributeName, string attributeValue)
        {
            return JsonConvert.DeserializeObject(ConnectToNeo.Instance.GetRelationsByObjectIdAndFillterRelationsWithParameter(labelName, objectId, attributeName, attributeValue));
        }


        public void CreateNode(string tableName,Dictionary<string,object> mongoValues, Dictionary<string, object> neoValues)
        {
            string objectId = Guid.NewGuid().ToString();
            mongoValues.Add("_id", objectId);
            neoValues.Add("_id", objectId);
            ConnectToMongo.Instance.CreateNode(tableName, mongoValues);
            ConnectToNeo.Instance.CreateNode(tableName, neoValues);
        }

        public void CreateRelation(string tableName,string sourceObjectId,string destObjectId,string relationName)
        {
            string objectId = ConnectToMongo.Instance.CreateRelation(tableName, sourceObjectId, destObjectId, relationName);
            ConnectToNeo.Instance.CreateRelation(tableName, objectId, sourceObjectId, destObjectId, relationName);
        }
  
        public void UpdateNodeByObjectID(string tableName,string objectId, Dictionary<string, object> mongoValues, Dictionary<string, object> neoValues)
        {
            ConnectToMongo.Instance.UpdateNode(tableName, objectId, mongoValues);
            ConnectToNeo.Instance.UpdateNode(tableName, objectId, neoValues);
        }

        public void DeleteNodeByObjectID(string objectId)
        {
            foreach (string currectRelations in ConnectToMongo.Instance.GetRelationsObjectIdsBySourceObjectId("Relations", "sourceObjectId", objectId))
            {
                ConnectToMongo.Instance.DeleteNodeByObjectID(currectRelations);
            }
            ConnectToMongo.Instance.DeleteNodeByObjectID(objectId);
            ConnectToNeo.Instance.DeleteNodeByObjectID(objectId);
        }

        #endregion

        #region Logs - ExecptionHandler

        public void ExecptionHandler(Exception exception)
        {

            Dictionary<string,object> dictValues = new Dictionary<string, object>();

            #region ErrorBuilder

            dictValues.Add("DateTime", DateTime.Now.ToString());
            dictValues.Add("Ip", LocalIPAddress());
            dictValues.Add("_id", Guid.NewGuid().ToString());
            dictValues.Add("name", exception.Message);

            #endregion

            ConnectToMongo.Instance.CreateNode("Logs", dictValues);
        }

        private string LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
        #endregion
    }
}
