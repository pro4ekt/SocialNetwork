using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Repositories;

namespace SocialNetwork.Tests.DAL_Test
{
    [TestClass]
    public class MessageManagerTests
    {
        private TestContext context;
        private MessageManager service;

        [TestInitialize]
        public void SetupContext()
        {
            context = new TestContext();
            service = new MessageManager(context);
        }

        [TestMethod]
        public async Task MessageManager_Create()
        {
            service.Create(new Messages{Id = "1", Text = "Test"});

            Assert.AreEqual(1, context.Messages.Count());
            Assert.AreEqual("1", context.Messages.Single().Id);
            Assert.AreEqual("Test", context.Messages.Single().Text);
        }

        [TestMethod]
        public async Task MessageManager_GetAll()
        {
            service.Create(new Messages { Id = "1", Text = "Test1" });
            service.Create(new Messages { Id = "2", Text = "Test2" });
            service.Create(new Messages { Id = "3", Text = "Test3", MessageId = "12312"});
            service.Create(new Messages { Id = "4", Text = "Test4" });

            var messages = service.GetAll();

            Assert.AreEqual(4, messages.Count);
            Assert.AreEqual("12312", messages[2].MessageId);
            Assert.AreEqual("Test2", messages[1].Text);
        }

        [TestMethod]
        public async Task ClientManager_Remove()
        {
            var message = new Messages {Id = "1",Text = "Aboba", MessageId = "testid"};
            service.Create(message);
            service.Remove(message);

            Assert.AreEqual(0, context.Messages.Count());
        }

        //[TestMethod]
        //public async Task ClientManager_Find()
        //{
        //    service.Create(new ClientProfile { Id = "1", Age = 5 });
        //    service.Create(new ClientProfile { Id = "2", Age = 6 });

        //    var client2 = service.Find("2");
        //    var clientNull = service.Find("3");

        //    Assert.IsNotNull(client2);
        //    Assert.IsNull(clientNull);
        //}
    }
}
