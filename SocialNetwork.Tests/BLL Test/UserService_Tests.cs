using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Infrastructure;
using BLL.Interfaces;
using BLL.Services;
using DAL.Repositories;

namespace SocialNetwork.Tests.BLL_Test
{
    [TestClass]
    public class UserService_Tests
    {
        private TestContext context;
        private UserService service;
        private IServiceCreator creator;

        [TestInitialize]
        public void SetupContext()
        {
            creator = new ServiceCreator();
            service = (UserService)creator.CreateUserService("FakeConnection");
        }

        [TestMethod]
        public async Task UserServise_SetInitialData_AND_FindAll()
        {
            var user1 = new UserDTO { Email = "user@gmail", Password = "qwerty", UserName = "Tester", Role = "user", Age = 5 };
            var user2 = new UserDTO { Email = "admin@gmail", Password = "qwerty", UserName = "Admin", Role = "admin", Age = 5 };
            List<string> roles = new List<string> { "user", "admin" };
            await service.SetInitialData(new List<UserDTO> { user1, user2 }, roles);
            var users = await service.FindAll();

            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
            Assert.AreEqual("Admin", users[1].UserName);
        }

        [TestMethod]
        public async Task UserServise_FindById()
        {
            var user = await service.FindById("1a38a12a-b13e-4ba4-a815-ed2b3429d3df");

            Assert.IsNotNull(user);
            Assert.AreEqual("Tester", user.UserName);
            Assert.AreEqual("user@gmail", user.Email);
            Assert.AreEqual("user", user.Role);
        }

        [TestMethod]
        public async Task UserServise_FindByEmail()
        {
            var user = await service.FindByEmail("user@gmail");

            Assert.IsNotNull(user);
            Assert.AreEqual("Tester", user.UserName);
            Assert.AreEqual("user@gmail", user.Email);
            Assert.AreEqual("user", user.Role);
        }

        [TestMethod]
        public async Task UserServise_FindByName()
        {
            var user = await service.FindByName("Tester");

            Assert.IsNotNull(user);
            Assert.AreEqual("Tester", user.UserName);
            Assert.AreEqual("user@gmail", user.Email);
            Assert.AreEqual("user", user.Role);
        }

        [TestMethod]
        public async Task UserServise_FindByAge()
        {
            var users = await service.FindByAge(5);

            Assert.IsNotNull(users);
            Assert.AreEqual(2,users.Count);
        }

        [TestMethod]
        public async Task UserServise_Authenticate()
        {
            var user1 = await service.FindByName("Tester");
            var claim1 = await service.Authenticate(user1);
            var user2 = await service.FindByName("Test1");
            var claim2 = await service.Authenticate(user2);

            Assert.IsNotNull(claim1);
            Assert.IsNull(claim2);
        }

        [TestMethod]
        public async Task UserServise_Create_AND_Remove()
        {
            var u = new UserDTO { Email = "a@gmail", Password = "abcdefg", UserName = "Aboba", Role = "user", Age = 6 };

            await service.Create(u);
            var u1 = await service.FindByName(u.UserName);
            await service.RemoveUser(u1.Id);
            var u2 = await service.FindByName(u1.UserName);

            Assert.IsNotNull(u1);
            Assert.IsNull(u2);
            Assert.AreEqual(u.Email, u1.Email);
        }

        [TestMethod]
        public async Task UserServise_BanUser_AND_UnBan()
        {
            var u = await service.FindByName("Tester");
            var b1 = await service.BanUser(u.Id);
            var b2 = await service.BanUser(u.Id);
            var b3 = await service.UnBanUser(u.Id);
            var b4 = await service.UnBanUser(u.Id);

            Assert.IsTrue(b1);
            Assert.IsFalse(b2);
            Assert.IsTrue(b3);
            Assert.IsFalse(b4);
        }

        [TestMethod]
        public async Task UserServise_EditProfile()
        {
            var u1 = await service.FindByName("Tester");
            var info1 = u1.Info;
            var b1 = await service.EditProfile(u1.Id, u1.UserName, u1.Email, u1.Info+1, u1.Address, u1.Age, u1);

            Assert.IsTrue(b1);
            Assert.IsNotNull(u1);
            Assert.AreNotEqual(info1, u1.Age);
        }

        [TestMethod]
        public async Task UserServise_AddFriend_AND_RemoveFriend()
        {
            var users = await service.FindAll();
            var b1 = await service.AddFriend(users[0].Id, users[0].Id);
            var b2 = await service.AddFriend(users[0].Id, users[1].Id);
            var b3 = await service.RemoveFriend(users[0].Id, users[1].Id);

            Assert.IsFalse(b1);
            Assert.IsTrue(b2);
            Assert.IsTrue(b3);
        }

        [TestMethod]
        public async Task UserServise_CheckPassword()
        {
            var b1 = await service.CheckPassword("user@gmail", "qwerty");
            var b2 = await service.CheckPassword("user@gmail", "qwerta");

            Assert.IsTrue(b1);
            Assert.IsFalse(b2);
        }

        [TestMethod]
        public async Task UserServise_SaveMessage_AND_RemoveMessage_AND()
        {
            MessagesDTO message1 = new MessagesDTO{ MessageId = "1", DateTime = DateTime.Today ,Text = "1", Id = "1a38a12a-b13e-4ba4-a815-ed2b3429d3df", ReceiverId = "eb852ca1-12fd-48c7-8133-761291b94cb5" };
            MessagesDTO message2 = new MessagesDTO{ MessageId = "2", DateTime = DateTime.Today ,Text = "2", Id = "eb852ca1-12fd-48c7-8133-761291b94cb5", ReceiverId = "1a38a12a-b13e-4ba4-a815-ed2b3429d3df" };
            var b1 = await service.SaveMessage(message1);
            var b2 = await service.SaveMessage(message2);
            var list = await service.GetAllMessages();
            var b3 = await service.RemoveMessage(message1);
            var b4 = await service.RemoveMessage(message2);

            Assert.IsNotNull(list);
            Assert.AreEqual(list[0].MessageId,message1.MessageId);
            Assert.AreEqual(list[1].MessageId, message2.MessageId);
            Assert.AreEqual(2,list.Count);
            Assert.IsTrue(b1);
            Assert.IsTrue(b2);
            Assert.IsTrue(b3);
            Assert.IsTrue(b4);
        }

    }
}
