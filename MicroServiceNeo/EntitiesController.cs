using FactoryNeoMongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MicroServiceNeo
{
    public enum Tables
    {
        Mongo,
        Neo
    }

    public class EntitiesController : ApiController
    {
        public EntitiesController()
        {
            DuplicateConnection.Instance.ConnectSettings();
        }


        #region Get Methods


        [Route("api/Entities/{dataBase}")]
        [HttpGet]
        public IHttpActionResult GetDataByQuery(Tables dataBase, string query)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        //return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetNeoDataByQuery(query));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}")]
        [HttpPost]
        public IHttpActionResult QueryGetData(Tables dataBase, [FromBody] Dictionary<string,string> queryData)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                    //return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetNeoDataByQuery(queryData["query"]));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }


        [Route("api/Entities/{dataBase}")]
        [HttpGet]
        public IHttpActionResult GetAllTablesNames(Tables dataBase)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok(DuplicateConnection.Instance.GetCollectionsNames);
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetLabelsNames);
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}")]
        [HttpGet]
        public IHttpActionResult GetAllDataInTableName(Tables dataBase, string tableName)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetDataByLabelName(tableName));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/{objectId}")]
        [HttpGet]
        public IHttpActionResult GetDataFromTableByObjectId(Tables dataBase, string tableName, string objectId)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok(DuplicateConnection.Instance.GetDataByCollectionNameFillteredWithAttributeName(tableName, new Dictionary<string, List<string>>() { { "_id", new List<string>() { objectId } } }));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetDataByLabelNameFillteredWithAttributeName(tableName, new Dictionary<string, List<string>>() { { "_id", new List<string>() { objectId } } }));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/{objectId}/Relations")]
        [HttpGet]
        public IHttpActionResult GetRelations(Tables dataBase, string tableName, string objectId)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                    // return Ok(DuplicateConnection.Instance.GetAllRelationsCollectionsBySourceObjectId(tableName, objectId));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetRelationsLabelsByObjectId(tableName, objectId));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/{objectId}/Relations/{objectDestId}")]
        [HttpGet]
        public IHttpActionResult GetRelationsByObjectId(Tables dataBase, string tableName, string objectId, string objectDestId)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok("In Patch");
                    // return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetRelationsByObjectIdAndFillterRelationsWithParameter(tableName, objectId, "_id", objectDestId));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/{objectId}/Relations/TableName/{relatedTableName}")]
        [HttpGet]
        public IHttpActionResult GetRelationByRelationTable(Tables dataBase, string tableName, string objectId, string relatedTableName)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok("In Patch");
                    // return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetRelationsByObjectIdAndFillterRelationsWithTableName(tableName, objectId, relatedTableName));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/{objectId}/Relations/{attributeName}/{attributeValue}")]
        [HttpGet]
        public IHttpActionResult GetRelationFillteredByAttribute(Tables dataBase, string tableName, string objectId, string attributeName, string attributeValue)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok("In Patch");
                    //return Ok(DuplicateConnection.Instance.GetDataByCollectionName(tableName));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetRelationsByObjectIdAndFillterRelationsWithParameter(tableName, objectId, attributeName, attributeValue));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{dataBase}/{tableName}/Fillter")]
        [HttpPost]
        public IHttpActionResult Post(Tables dataBase, string tableName, [FromBody]Dictionary<string, List<string>> fillterNameWithValues)
        {
            try
            {
                switch (dataBase)
                {
                    case Tables.Mongo:
                        return Ok(DuplicateConnection.Instance.GetDataByCollectionNameFillteredWithAttributeName(tableName, fillterNameWithValues));
                    case Tables.Neo:
                        return Ok(DuplicateConnection.Instance.GetDataByLabelNameFillteredWithAttributeName(tableName, fillterNameWithValues));
                    default:
                        return Ok("No Such An DataBase You Have Asked For.");
                }
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }


        #endregion

        #region Create Methods

        [Route("api/Entities/{tableName}/CreateEntity")]
        [HttpPost]
        public IHttpActionResult Post(string tableName, [FromBody]Dictionary<string, Dictionary<string, object>> values)
        {
            try
            {
                DuplicateConnection.Instance.CreateNode(tableName, values[Tables.Mongo.ToString()], values[Tables.Neo.ToString()]);
                return Ok();
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        [Route("api/Entities/{tableName}/{sourceObjectId}/{destObjectId}/{relationName}")]
        [HttpGet]
        public IHttpActionResult Get(string tableName, string sourceObjectId, string destObjectId, string relationName)
        {
            try
            {
                DuplicateConnection.Instance.CreateRelation(tableName, sourceObjectId, destObjectId, relationName);
                return Ok();
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
                return Ok("You Might Do Something Wrong? :O");
            }
        }

        #endregion

        #region Update Methods

        [Route("api/Entities/{tableName}/{objectId}")]
        [HttpPost]
        public void Put(string tableName, string objectId, [FromBody]Dictionary<string, Dictionary<string, object>> values)
        {
            try
            {
                DuplicateConnection.Instance.UpdateNodeByObjectID(tableName, objectId, values[Tables.Mongo.ToString()], values[Tables.Neo.ToString()]);

            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
            }
        }

        #endregion

        #region Delete Methods

        [Route("api/Entities/Delete/{objectId}")]
        [HttpGet]
        public void Delete(string objectId)
        {
            try
            {
                DuplicateConnection.Instance.DeleteNodeByObjectID(objectId);
            }
            catch (Exception execption)
            {
                DuplicateConnection.Instance.ExecptionHandler(execption);
            }
        }

        #endregion

    }
}
