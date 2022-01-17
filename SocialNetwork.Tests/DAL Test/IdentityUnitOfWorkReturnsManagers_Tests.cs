using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DAL.Entities;
using DAL.Repositories;

namespace SocialNetwork.Tests.DAL_Test
{
    [TestClass]
    public class IdentityUnitOfWorkTests
    {
        [TestMethod]
        public void IdentityUnitOfWorkReturnsManagers()
        {
            IdentityUnitOfWork i = new IdentityUnitOfWork(@"Data Source=(LocalDB)\MSSQLLocalDB;Database = Fake;Integrated Security=True;Persist Security Info = True");

            var c = (ClientManager)i.ClientManager;
            var f = i.FriendsManager;
            var m = i.MessageManager;
            var a = c.Database;

            Assert.IsNotNull(c.Database);
            Assert.IsNotNull(f);
            Assert.IsNotNull(m);
        }
    }
}
