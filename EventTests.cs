using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Xunit;
using EventManagementAPI.Data;
using EventManagementAPI.Controllers;
using EventManagementAPI.Models;

namespace EventManagementAPI.Tests
{
    public class EventTests
    {
        private AppDbContext GetDb() => new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(System.Guid.NewGuid().ToString()).Options);

        private DefaultHttpContext GetContext(string role = "Organiser", int userId = 1)
        {
            var ctx = new DefaultHttpContext();
            var sess = new FakeSession();
            sess.SetInt32("UserId", userId);
            sess.SetString("UserRole", role);
            ctx.Session = sess;
            return ctx;
        }

        [Fact]
        public async Task Organiser_Can_Create_Event()
        {
            var db = GetDb();
            db.Users.Add(new User{Id=1, Email="org@test.com", Role=UserRole.Organiser, PasswordHash="x", FirstName="O", LastName="R"});
            await db.SaveChangesAsync();
            var ctrl = new EventsController(db){ControllerContext=new ControllerContext{HttpContext=GetContext("Organiser")}};
            var result = await ctrl.Create(new CreateEventDto("Run 10k","Fun run",DateTime.Now.AddDays(10),"Pretoria CBD",10,"run",null,null));
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Participant_Cannot_Create_Event()
        {
            var db = GetDb();
            var ctrl = new EventsController(db){ControllerContext=new ControllerContext{HttpContext=GetContext("Participant")}};
            var result = await ctrl.Create(new CreateEventDto("Run","desc",DateTime.Now,"loc",5,"run",null,null));
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Enrolment_Should_Be_Recorded()
        {
            var db = GetDb();
            db.Users.Add(new User{Id=1, Email="p@test.com", Role=UserRole.Participant, PasswordHash="x", FirstName="P", LastName="T"});
            db.Events.Add(new Event{Id=1, Name="Run", Location="Pretoria", Distance=10, EventType="run", EventDate=DateTime.Now, CreatedByUserId=1});
            await db.SaveChangesAsync();
            var ctx = GetContext("Participant", 1);
            var enrolCtrl = new EnrolmentsController(db){ControllerContext=new ControllerContext{HttpContext=ctx}};
            var result = await enrolCtrl.Enrol(new CreateEnrolmentDto(1,null));
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Single(db.Enrolments);
        }

        [Fact]
        public async Task Organiser_Can_View_Enrolments_For_Event()
        {
            var db = GetDb();
            db.Enrolments.Add(new Enrolment{Id=1, EventId=1, UserId=1});
            await db.SaveChangesAsync();
            var ctrl = new EnrolmentsController(db){ControllerContext=new ControllerContext{HttpContext=GetContext("Organiser")}};
            var result = await ctrl.GetByEvent(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
