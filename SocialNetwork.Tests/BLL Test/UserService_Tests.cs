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
            service = (UserService)creator.CreateUserService(@"Data Source=(LocalDB)\MSSQLLocalDB;Database = Fake;Integrated Security=True;Persist Security Info = True");
        }

        [TestMethod]
        public async Task UserServise_Create()
        {
            var user = new UserDTO { Email = "test@gmail", Password = "qwerty", UserName = "Tester", Role = "user", Address = "S", Info = "add" };
            List<string> roles = new List<string> { "user", "admin" };
            await service.SetInitialData(new List<UserDTO> {user}, roles);

            Assert.IsNull(null);
        }
    }
}
