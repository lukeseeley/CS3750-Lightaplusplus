using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.BisLogic;

namespace UnitTestLMSProject
{
    [TestClass]
    public class RegistrationUnitTestClass
    {
        [TestMethod]
        public void RegisterTest()
        {
            //Prep Stage
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);

            StudentRegister register = new StudentRegister(context);
            if (register.checkRegistration(1017, 1008)) register.DropStudent(1017, 1008);

            /////Unit Test Instructor Id = 1016; Section Id = 1008; Unit Test Student = 1017;
            // Act
            register.RegisterStudent(1017, 1008);

            //Assert
            Assert.IsTrue(register.checkRegistration(1017, 1008));
            Assert.IsFalse(register.checkRegistration(1017, -1));

            //Clean Up
            register.DropStudent(1017, 1008);
        }

        [TestMethod]
        public void DropTest()
        {
            var options = new DbContextOptionsBuilder<Lightaplusplus.Data.Lightaplusplus_SystemContext>();
            options.UseSqlServer(UnitTestConfig.ConnectionString);
            var context = new Lightaplusplus.Data.Lightaplusplus_SystemContext(options.Options);

            StudentRegister register = new StudentRegister(context);
            if (!register.checkRegistration(1017, 1008)) register.DropStudent(1017, 1008);

            /////Unit Test Instructor Id = 1016; Section Id = 1008; Unit Test Student = 1017;
            // Act
            register.DropStudent(1017, 1008);

            //Assert
            Assert.IsFalse(register.checkRegistration(1017, 1008));
        }
    }
}