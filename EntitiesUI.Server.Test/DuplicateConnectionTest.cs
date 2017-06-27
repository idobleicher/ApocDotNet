using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FactoryNeoMongo;
using System.Collections.Generic;
using MongoDAL;
using NeoDAL;

namespace EntitiesUI.Server.Test
{
    
    [TestClass]
    public class DuplicateConnectionTest
    {
        public DuplicateConnectionTest()
        {
            ConnectToMongo.Instance.SetConnectionSettings("mongodb://localhost:27017/", "test");
            ConnectToNeo.Instance.SetConnectionSettings("Bolt://localhost:7687", "neo4j", "nv1vnmc2");
        }

        [TestMethod]
        public void CreateNodeTest()
        {   
            DuplicateConnection.Instance.CreateNode("Persons", new Dictionary<string, object>() { { "ido", "hamor" } }, new Dictionary<string, object>() { { "ido", "hamor" } });
            Assert.Fail();
        }
    }
}
