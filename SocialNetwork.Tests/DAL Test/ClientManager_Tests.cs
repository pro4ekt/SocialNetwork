using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace SocialNetwork.Tests.DAL_Test
{
    [TestClass]
    public class ClientManagerTests
    {
        private TestContext context;
        private ClientManager service;

        [TestInitialize]
        public void SetupContext()
        {
            context = new TestContext("FakeConnection");
            service = new ClientManager(context);
        }

        [TestMethod]
        public async Task ClientManager_Create()
        {
            service.Create(new ClientProfile { Id = "1", Age = 5 });

            Assert.AreEqual(1, context.ClientProfiles.Count());
            Assert.AreEqual("1", context.ClientProfiles.Single().Id);
            Assert.AreEqual(5, context.ClientProfiles.Single().Age);
        }

        [TestMethod]
        public async Task ClientManager_GetAll()
        {
            service.Create(new ClientProfile { Id = "1", Age = 5, ApplicationUser = new ApplicationUser() {UserName = "Test"}});
            service.Create(new ClientProfile { Id = "2", Age = 6 });
            service.Create(new ClientProfile { Id = "3", Age = 5 });
            service.Create(new ClientProfile { Id = "4", Age = 7 });

            var clients = service.GetAll();

            Assert.AreEqual(4, clients.Count);
            Assert.AreEqual(6, clients[1].Age);
            Assert.AreEqual("Test", clients[0].ApplicationUser.UserName);
        }

        [TestMethod]
        public async Task ClientManager_Remove()
        {
            var client = new ClientProfile {Id = "1", Age = 5};
            service.Create(client);
            service.Remove(client);

            Assert.AreEqual(0, context.ClientProfiles.Count());
        }

        [TestMethod]
        public async Task ClientManager_Find()
        {
            service.Create(new ClientProfile { Id = "1", Age = 5 });
            service.Create(new ClientProfile { Id = "2", Age = 6 });

            var client2 = service.Find("2");
            var clientNull = service.Find("3");

            Assert.IsNotNull(client2);
            Assert.IsNull(clientNull);
        }
    }
}
