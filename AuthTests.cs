using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using EventManagementAPI.Data;
using EventManagementAPI.Controllers;
using EventManagementAPI.Models;
using EventManagementAPI.Helpers;

namespace EventManagementAPI.Tests
{
    public class AuthTests
    {
        private AppDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options;
            return new AppDbContext(options);
        }

        private DefaultHttpContext GetHttpContext()
        {
            var ctx = new DefaultHttpContext();
            ctx.Session = new FakeSession();
            return ctx;
        }

        [Fact]
        public async Task Register_ShouldHashPassword_AndReturn201()
        {
            var db = GetDb();
            var ctrl = new AuthController(db)
            {
                ControllerContext = new ControllerContext { HttpContext = GetHttpContext() }
            };

            var result = await ctrl.Register(new RegisterDto("John","Doe","john@test.com","Pass123!","Organiser"));

            Assert.IsType<CreatedAtActionResult>(result);
            var user = await db.Users.FirstAsync();
            Assert.NotEqual("Pass123!", user.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("Pass123!", user.PasswordHash));
        }

        [Fact]
        public async Task Login_ShouldFail_WithWrongPassword()
        {
            var db = GetDb();
            var ctx = GetHttpContext();
            var ctrl = new AuthController(db)
            {
                ControllerContext = new ControllerContext { HttpContext = ctx }
            };

            await ctrl.Register(new RegisterDto("A","B","a@b.com","123456","Participant"));
            var login = await ctrl.Login(new LoginDto("a@b.com","wrong"));

            Assert.IsType<UnauthorizedObjectResult>(login);
        }

        [Fact]
        public async Task Login_ShouldSucceed_WithCorrectPassword()
        {
            var db = GetDb();
            var ctx = GetHttpContext();
            var ctrl = new AuthController(db)
            {
                ControllerContext = new ControllerContext { HttpContext = ctx }
            };

            await ctrl.Register(new RegisterDto("A","B","a@b.com","123456","Participant"));
            var login = await ctrl.Login(new LoginDto("a@b.com","123456"));

            Assert.IsType<OkObjectResult>(login);
        }

        [Fact]
        public void RoleBased_Access_Check()
        {
            var ctx = GetHttpContext();
            ctx.Session.SetInt32("UserId", 1);
            ctx.Session.SetString("UserRole", "Participant");

            Assert.False(SessionHelper.IsOrganiser(ctx.Session));
            Assert.True(SessionHelper.IsParticipant(ctx.Session));
            Assert.True(SessionHelper.IsAuthenticated(ctx.Session));
        }
    }
}
