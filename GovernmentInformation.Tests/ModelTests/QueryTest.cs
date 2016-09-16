using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using GovernmentInformation.Models;

namespace GovernmentInformation.Tests
{
    public class QueryTest
    {
        [Fact]
        public void GetDescriptionTest()
        {
            //Arrange
            var query = new Query();
            query.Description = "Is dog clean?";

            //Act
            var result = query.Description;

            //Assert
            Assert.Equal("Is dog clean?", result);
        }
    }
}