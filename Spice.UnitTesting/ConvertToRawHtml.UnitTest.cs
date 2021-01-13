using Microsoft.AspNetCore.Http;
using Moq;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System;
using System.Collections.Generic;
using Xunit;


namespace Spice.UnitTesting
{
    public class ConvertToRawHtmlTest
    {
        [Fact]
        public void ConvertToRawHtml_EmptySource_ReturnTrue()
        {
            string source = "";
            Assert.True(SD.ConvertToRawHtml(source) == "");
        }
        [Fact]
        public void ConvertToRawHtml_IsLessThanSource_ReturnTrue()
        {
            string source = "<";
            Assert.True(SD.ConvertToRawHtml(source) == "");
        }
        [Fact]
        public void ConvertToRawHtml_IsMoreThanSource_ReturnTrue()
        {
            string source = ">";
            Assert.True(SD.ConvertToRawHtml(source) == "");
        }
        [Fact]
        public void ConvertToRawHtml_NotHaveIsMoreThanSource_ReturnTrue()
        {
            string source = "<h1";
            Assert.True(SD.ConvertToRawHtml(source) == "");
        }
        [Fact]
        public void ConvertToRawHtml_WordSource_ReturnTrue()
        {
            string source = "Hello";
            Assert.True(SD.ConvertToRawHtml(source) == "Hello");
        }
        [Fact]
        public void ConvertToRawHtml_FullSource_ReturnTrue()
        {
            string source = "<h1>Hello</h1><p> My name is John</p>";
            Assert.True(SD.ConvertToRawHtml(source) == "Hello My name is John");
        }
    }
}
