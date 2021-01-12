using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Repositories;
using Xunit;

namespace TestApp
{
    [TestClass]
    public class UnitTest_ConvertToRawHtml
    {
        [TestMethod]
        public void ConvertToRawHtml_EmptySource_ReturnTrue()
        {
            string source = "";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "");
        }
        [TestMethod]
        public void ConvertToRawHtml_IsLessThanSource_ReturnTrue()
        {
            string source = "<";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "");
        }
        [TestMethod]
        public void ConvertToRawHtml_IsMoreThanSource_ReturnTrue()
        {
            string source = ">";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "");
        }
        [TestMethod]
        public void ConvertToRawHtml_NotHaveIsMoreThanSource_ReturnTrue()
        {
            string source = "<h1";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "");
        }
        [TestMethod]
        public void ConvertToRawHtml_WordSource_ReturnTrue()
        {
            string source = "Hello";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "Hello");
        }
        [TestMethod]
        public void ConvertToRawHtml_FullSource_ReturnTrue()
        {
            string source = "<h1>Hello</h1><p> My name is John</p>";
            Assert.IsTrue(SD.ConvertToRawHtml(source) == "Hello My name is John");
        }
    }
}
