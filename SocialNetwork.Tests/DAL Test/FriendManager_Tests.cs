using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace SocialNetwork.Tests.DAL_Test
{
    [TestClass]
    public class FriendManagerTests
    {
        private TestContext context;
        private FriendsManager service;

        [TestInitialize]
        public void SetupContext()
        {
            context = new TestContext(@"Data Source=(LocalDB)\MSSQLLocalDB;Database = Fake;Integrated Security=True;Persist Security Info = True");
            service = new FriendsManager(context);
        }

        [TestMethod]
        public async Task FriendsManager_Create()
        {
            service.Create(new Friends{ Id = "1", FriendId = "2"});
            var friends = context.Friends.ToList();

            Assert.AreEqual(2, context.Friends.Count());
            Assert.AreEqual("2", friends[1].Id);
        }

        [TestMethod]
        public async Task FriendsManager_GetAll()
        {
            service.Create(new Friends { Id = "1", FriendId = "2" });
            service.Create(new Friends { Id = "3", FriendId = "4" });
            service.Create(new Friends { Id = "5", FriendId = "6" });

            var friends = service.GetAll();

            Assert.AreEqual(6, friends.Count);
            Assert.AreEqual("4",friends[3].Id);
        }

        [TestMethod]
        public async Task FriendsManager_Remove()
        {
            var friend = new Friends { Id = "1", FriendId = "2" };
            service.Create(friend);
            service.Remove(friend);

            Assert.AreEqual(0, context.Friends.Count());
        }
    }
}
