using Neo4j.Driver.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoDAL
{
    public class ConnectToNeo : IDisposable
    {
        #region DataMembers

        private IDriver _database { get; set; }
        private ISession _session { get; set; }

        #endregion

        #region Singleton

        private static ConnectToNeo instance = null;

        private ConnectToNeo()
        {

        }

        public static ConnectToNeo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectToNeo();
                }

                return instance;
            }
        }

        #endregion

        #region Settings/Connection

        public void SetConnectionSettings(string connectionString, string username, string password)
        {
            _database = GraphDatabase.Driver(connectionString, AuthTokens.Basic(username, password), Config.DefaultConfig);
        }

        public void Connect()
        {
            _session = _database.Session();
        }

        public void Dispose()
        {
            _session?.Dispose();
        }

        #endregion

        #region CrudNeoConnect

        public string GetDataByQuery(string query)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run(query + " as nodes");

            foreach (IRecord node in results)
            {
                if ((node["nodes"] as INode) != null)
                    Data.Add((node["nodes"] as INode).Properties);
            }

            IStatementResult relationsFinder = ConnectToNeo.Instance._session.Run(query + " as nodes");

            foreach (IRecord node in relationsFinder)
            {
                if ((node["nodes"] as IRelationship) != null)
                    Data.Add((node["nodes"] as IRelationship).Properties);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetLabelsNames()
        {
            var Data = new Dictionary<string,object>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run($"MATCH (n) RETURN distinct labels(n)[0] as tables,  count(*) as tablesObjectNumbers union MATCH ()-[x]->()   return  'Relations' as tables, count(x) as tablesObjectNumbers");

            foreach (IRecord node in results)
            {
                string tableName = (node[node.Keys[0]]).ToString();
                object tableCount = (node[node.Keys[1]] as object).ToString();
                Data.Add(tableName, tableCount);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetDataByLabelName(string LabelName)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run($"MATCH(n:{LabelName} ) return(n) as nodes");

            foreach (IRecord node in results)
            {
                Data.Add((node["nodes"] as INode).Properties);
            }

            IStatementResult relationsFinder = ConnectToNeo.Instance._session.Run($"MATCH() - [r:{LabelName}] - () RETURN DISTINCT(r) as nodes");

            foreach (IRecord node in relationsFinder)
            {
                Data.Add((node["nodes"] as IRelationship).Properties);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetDataByLabelNameFillteredWithAttributeName(string LabelName, Dictionary<string, List<string>> fillterNameWithValues)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            #region NodeFillter

            IStatementResult results = ConnectToNeo.Instance._session.Run($"MATCH(n:{LabelName} ) return(n) as nodes");

            bool bToAdd = true;
            foreach (IRecord node in results)
            {
                foreach (string attributeName in fillterNameWithValues.Keys)
                {
                    if ((node["nodes"] as INode).Properties.Keys.Contains(attributeName) && !(fillterNameWithValues[attributeName].Contains((node["nodes"] as INode).Properties[attributeName])))
                    {
                        bToAdd = false;
                    }
                }

                if (bToAdd)
                {
                    Data.Add((node["nodes"] as INode).Properties);
                }

                bToAdd = true;
            } 
            #endregion

            #region RelationFillter

            IStatementResult relationsFinder = ConnectToNeo.Instance._session.Run($"MATCH() - [r:{LabelName}] - () RETURN (r) as nodes");

            foreach (IRecord node in relationsFinder)
            {
                foreach (string attributeName in fillterNameWithValues.Keys)
                {
                    if ((node["nodes"] as IRelationship).Properties.Keys.Contains(attributeName) && !(fillterNameWithValues[attributeName].Contains((node["nodes"] as IRelationship).Properties[attributeName])))
                    {
                        bToAdd = false;
                    }
                }

                if (bToAdd)
                {
                    Data.Add((node["nodes"] as IRelationship).Properties);
                }

                bToAdd = true;
            } 
            #endregion

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetRelationsByObjectId(string labelName,string objectId)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run($"match(n:{labelName})-[x]-(r) where n._id = '{objectId}'  return r as nodes");

            foreach (IRecord node in results)
            {
                Data.Add((node["nodes"] as INode).Properties);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetRelationsByObjectIdAndFillterRelationsWithTableName(string labelName, string objectId, string relatedTableName)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run($"match(n:{labelName})-[x]-(r:{relatedTableName}) where n._id = '{objectId}' return r as nodes");

            foreach (IRecord node in results)
            {
                Data.Add((node["nodes"] as INode).Properties);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public string GetRelationsByObjectIdAndFillterRelationsWithParameter(string labelName,string objectId, string attributeName, string attributeValue)
        {
            var Data = new List<IReadOnlyDictionary<string, object>>();

            Connect();

            IStatementResult results = ConnectToNeo.Instance._session.Run($"match(n:{labelName})-[x]-(r) where n._id = '{objectId}' AND r.{attributeName} = '{attributeValue}'  return r as nodes");

            foreach (IRecord node in results)
            {
                Data.Add((node["nodes"] as INode).Properties);
            }

            Dispose();

            return JsonConvert.SerializeObject(Data);
        }

        public void CreateNode(string labelName, Dictionary<string, object> values)
        {
            Connect();

            #region Building The Query

            StringBuilder query = new StringBuilder($"CREATE(:{labelName}" + "{");

            foreach (string key in values.Keys)
            {
                query.Append(key.ToString() + ":'" + values[key].ToString() + "',");
            }

            query.Remove(query.Length - 1, 1);
            query.Append("})");

            #endregion

            ConnectToNeo.Instance._session.Run(query.ToString());

            Dispose();
        }

        public void CreateRelation(string tableName,string objectId, string sourceObjectId, string destObjectId, string relationName)
        {
            Connect();
          
            ConnectToNeo.Instance._session.Run("MATCH(sourceNode{_id:" + $"'{sourceObjectId}'" + "}),(destNode{_id:" + $"'{destObjectId}'" + "})" + $" Create(sourceNode) -[:{tableName}" +"{"+$"name:'{relationName}'," +$"_id:'{objectId}'"+"}]->(destNode)");

            Dispose();
        }

        public void UpdateNode(string collectionName, string objectId, Dictionary<string, object> values)
        {
            Connect();
                
            #region Building The Query For Nodes

            StringBuilder query = new StringBuilder($"MATCH(node:{collectionName}) WHERE node._id = '{objectId}' SET ");

            foreach (string key in values.Keys)
            {
                query.Append("node."+key.ToString() + "='" + values[key].ToString() + "',");
            }

            query.Remove(query.Length - 1, 1);

            #endregion

            ConnectToNeo.Instance._session.Run(query.ToString());

            #region Building The Query For Relations

            query = new StringBuilder($"MATCH(() -[node:{collectionName}] -()) WHERE node._id = '{objectId}' SET ");

            foreach (string key in values.Keys)
            {
                query.Append("node." + key.ToString() + "='" + values[key].ToString() + "',");
            }

            query.Remove(query.Length - 1, 1);

            #endregion

            ConnectToNeo.Instance._session.Run(query.ToString());

            Dispose();
        }

        public void DeleteNodeByObjectID(string objectId)
        {
            Connect();

            ConnectToNeo.Instance._session.Run($"MATCH(n) where n._id = '{objectId}' detach delete(n)");

            ConnectToNeo.Instance._session.Run($"MATCH() - [relation] - () WHERE relation._id = '{objectId}' detach delete(relation)");

            Dispose();
        }

        #endregion
    }
}
