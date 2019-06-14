using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace User.api.UnitTests
{
    public class UserControllerUnitTests
    {
        private User.API.Data.UserContext GetUserContext()
        {
           var options = new DbContextOptionsBuilder<API.Data.UserContext>().Options;
            //.UseInMemoryDatabase(Guid.NewGuid().ToString());
            
            var userContext = new API.Data.UserContext(options);
            userContext.Users.Add(new API.Models.AppUser
            {
                Id = Guid.NewGuid(),
                Name = "xcz"
            });
            userContext.SaveChanges();
            return userContext;
        }

        [Fact]
        public async Task Get_ReturnRigthUser_WithExpectedParameters()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<API.Controllers.UserController>>();
            var logger = loggerMoq.Object;
            var controller = new User.API.Controllers.UserController(context, logger);
            var response = await controller.Get();
            Assert.IsType<JsonResult>(response);
        }
    }
}
