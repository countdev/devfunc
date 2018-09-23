using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhyNotRun.DAO;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace WhyNotRun.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private PublicationDAO _publicationDAO;

        public UnitTest1()
        {
            _publicationDAO = new PublicationDAO();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var a = await _publicationDAO.SeeMoreComments(new ObjectId("5a04815353d3576a4479b450"), new ObjectId("5a09c93c53d3570ca885ff16"), 10);
            
        }
    }
}
