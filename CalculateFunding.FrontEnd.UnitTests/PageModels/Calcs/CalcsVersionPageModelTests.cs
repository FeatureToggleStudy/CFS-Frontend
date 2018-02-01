﻿using AutoMapper;
using CalculateFunding.Frontend.Clients;
using CalculateFunding.Frontend.Clients.CalcsClient.Models;
using CalculateFunding.Frontend.Helpers;
using CalculateFunding.Frontend.Interfaces.ApiClient;
using CalculateFunding.Frontend.Pages.Calcs;
using CalculateFunding.Frontend.Services;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using CalculateFunding.Frontend.ViewModels.Calculations;
using CalculateFunding.Frontend.Clients.Models;
using System.Linq;
namespace CalculateFunding.Frontend.PageModels.Calcs
{
    [TestClass]
    public class CalcsVersionPageModelTests
    {
        [TestMethod]
        public async Task OnGet_WhenCalculationDoesNotExistThenNotFoundReturned()
        {
            // Arrange

            ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
            ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();
            IMapper mapper = MappingHelper.CreateFrontEndMapper();

            ILogger logger = Substitute.For<ILogger>();

            Calculation expectedCalculation = null;
            calcsClient
                .GetCalculationById(Arg.Any<string>())
                .Returns(new ApiResponse<Calculation>(System.Net.HttpStatusCode.NotFound, expectedCalculation));

            string calculationId = "1";

            ComparePageModel comparePageModel = new ComparePageModel(specsClient, calcsClient, mapper);

            // Act
            IActionResult result = await comparePageModel.OnGet(calculationId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        //public async Task OnGet_WhenSpecCalculationDoesNotExistThenNotFoundReturned()
        //{
        //    // Arrange
        //    ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
        //    ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();
        //    IMapper mapper = MappingHelper.CreateFrontEndMapper();

        //    ILogger logger = Substitute.For<ILogger>();

        //    string calculationId = "£$%";

        //    calcsClient.GetCalculationById(calculationId).Returns(new ApiResponse<Calculation>(System.Net.HttpStatusCode.OK, new Calculation()
        //    {
        //        Id = calculationId,
        //        CalculationSpecification = new Frontend.Clients.Models.Reference("1", "Test Calculation Specification"),
        //        SpecificationId = "54",
        //    }));


        //    ComparePageModel pageModel = new ComparePageModel(specsClient, calcsClient, mapper);
        //    // Act
        //    IActionResult result = await pageModel.OnGet(calculationId);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType<NotFoundObjectResult>();
        //}


        [TestMethod]
        public async Task OnGet_WhenGetAllVersionsbyCalculationIdReturnsNull_ThenServerErrorResponseIsReturned()
        {
            // Arrange
            ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
            ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();

            ILogger logger = Substitute.For<ILogger>();
            IMapper mapper = MappingHelper.CreateFrontEndMapper();

            string calculationID = "1";

            calcsClient
                .GetAllVersionsByCalculationId(calculationID)
                .Returns((ApiResponse<IEnumerable<CalculationVersion>>)null);

            ComparePageModel compPageModel = new ComparePageModel(specsClient, calcsClient, mapper);

            // Act

            IActionResult result = await compPageModel.OnGet(calculationID);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            StatusCodeResult typedResult = result as StatusCodeResult;
            typedResult.StatusCode.Should().Be(500);
        }


        [TestMethod]
        public async Task OnGet_WhenCalculationIDIsNullGetAllVersionsbyCalculationIdReturns_ThenServerErrorResponseIsReturned()
        {

            // Arrange
            ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
            ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();

            ILogger logger = Substitute.For<ILogger>();
            IMapper mapper = MappingHelper.CreateFrontEndMapper();
            string calculationID = null;

            calcsClient
                .GetAllVersionsByCalculationId(calculationID)
                    .Returns((ApiResponse<IEnumerable<CalculationVersion>>)null);

            ComparePageModel compPageModel = new ComparePageModel(specsClient, calcsClient, mapper);

            // Act
            IActionResult result = await compPageModel.OnGet(calculationID);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task OnGet_WhenSpecificationDoesntExistThenErrorReturned()
        {
            ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
            ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();
            IMapper mapper = MappingHelper.CreateFrontEndMapper();
            ILogger logger = Substitute.For<ILogger>();
            string calculationId = "1";
            Calculation expectedCalculation = new Calculation()
            {
                Id = "2",
                Name = "Specs Calculation",
                PeriodName = "2018/19",
                SpecificationId = "3",
                Status = "Draft",
                LastModifiedBy = new Reference("1", "Matt Vallily"),
                SourceCode = "Public Function GetProductResult(rid As String) As Decimal 'change to As String if text product     Dim result As Decimal = 0 'change to As String if text product     Dim P04_Learners As Decimal = products.1819_Additional_Funding.P04_Learner_Numbers     Dim P03_Rate As Decimal = products.1819_Additional_Funding.P03_Maths_Top_Up_Rate     result = P03_Rate * P04_learners     Return result End Function",
                Version = 4
            };

            calcsClient
                .GetCalculationById("2")
                .Returns(new ApiResponse<Calculation>(System.Net.HttpStatusCode.OK, expectedCalculation));

            Clients.SpecsClient.Models.Calculation specCalculation = new Clients.SpecsClient.Models.Calculation()
            {
                Id = "1",
                Name = "Test spec",
                Description = "Test description",
                AllocationLine = new Reference("1", "Test Allocation")
            };

            specsClient
            .GetCalculationById(calculationId, "3")
            .Returns(new ApiResponse<CalculateFunding.Frontend.Clients.SpecsClient.Models.Calculation>(System.Net.HttpStatusCode.NotFound, specCalculation));

            ComparePageModel compPageModel = new ComparePageModel(specsClient, calcsClient, mapper);

            // Act
            IActionResult result = await compPageModel.OnGet(calculationId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StatusCodeResult>();
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
        }



        [TestMethod]
        public async Task OnGet_WhenCalculationExistsThenCalculationReturned()
        {
            // Arrange
            ICalculationsApiClient calcsClient = Substitute.For<ICalculationsApiClient>();
            ISpecsApiClient specsClient = Substitute.For<ISpecsApiClient>();
            IMapper mapper = MappingHelper.CreateFrontEndMapper();

            ILogger logger = Substitute.For<ILogger>();

            string calculationId = "1";

            Calculation expectedCalculation = new Calculation()
            {
                Id = "1",
                Name = "Specs Calculation",
                PeriodName = "2018/19",
                SpecificationId = "3",
                Status = "Draft",
                LastModifiedBy = new Reference("1", "Matt Vallily"),
                SourceCode = "Public Function GetProductResult(rid As String) As Decimal 'change to As String if text product     Dim result As Decimal = 0 'change to As String if text product     Dim P04_Learners As Decimal = products.1819_Additional_Funding.P04_Learner_Numbers     Dim P03_Rate As Decimal = products.1819_Additional_Funding.P03_Maths_Top_Up_Rate     result = P03_Rate * P04_learners     Return result End Function",
                Version = 4,
                CalculationSpecification = new Reference("1", "Test Spec")
            };

            calcsClient
                .GetCalculationById(calculationId)
                .Returns(new ApiResponse<Calculation>(System.Net.HttpStatusCode.OK, expectedCalculation));


            Clients.SpecsClient.Models.Calculation specCalculation = new Clients.SpecsClient.Models.Calculation()
            {
                Id = "1",
                Name = "Test spec",
                Description = "Test description",
                AllocationLine = new Reference("1", "Test Allocation")
            };

            specsClient
           .GetCalculationById(expectedCalculation.SpecificationId, calculationId)
           .Returns(new ApiResponse<CalculateFunding.Frontend.Clients.SpecsClient.Models.Calculation>(System.Net.HttpStatusCode.OK, specCalculation));

            CalculationVersion calcsVersion1 = new CalculationVersion()
            {

                Status = "Draft",
                Version = "1",
                DecimalPlaces = 4,
                Date = new DateTime(2018, 1, 1, 12, 34, 45, 03),
                Author = new Reference("1", "Matt Vallily"),
                SourceCode = "Public Function GetProductResult(rid As String) As Decimal 'change to As String if text product     Dim result As Decimal = 0 'change to As String if text product     Dim P04_Learners As Decimal = products.1819_Additional_Funding.P04_Learner_Numbers     Dim P03_Rate As Decimal = products.1819_Additional_Funding.P03_Maths_Top_Up_Rate     result = P03_Rate * P04_learners     Return result End Function",
            };

            CalculationVersion calcsVersion2 = new CalculationVersion()
            {
                Status = "Draft",
                Version = "2",
                DecimalPlaces = 4,
                Date = new DateTime(2018, 1, 2, 12, 34, 45, 03),
                Author = new Reference("1", "Matt Vallily"),
                SourceCode = "Public Function GetProductResult(rid As String) As Decimal 'change to As String if text product     Dim result As Decimal = 0 'change to As String if text product     Dim P04_Learners As Decimal = products.1819_Additional_Funding.P04_Learner_Numbers     Dim P03_Rate As Decimal = products.1819_Additional_Funding.P03_Maths_Top_Up_Rate     result = P03_Rate * P04_learners     Return result End Function",
            };

            CalculationVersion calcsVersion3 = new CalculationVersion()
            {

                Status = "Draft",
                Version = "3",
                DecimalPlaces = 4,
                Date = new DateTime(2018, 1, 3, 12, 34, 45, 03),
                Author = new Reference("1", "Matt Vallily"),
                SourceCode = "Public Function GetProductResult(rid As String) As Decimal 'change to As String if text product     Dim result As Decimal = 0 'change to As String if text product     Dim P04_Learners As Decimal = products.1819_Additional_Funding.P04_Learner_Numbers     Dim P03_Rate As Decimal = products.1819_Additional_Funding.P03_Maths_Top_Up_Rate     result = P03_Rate * P04_learners     Return result End Function",
            };

            IEnumerable<CalculationVersion> calculationVersion = new List<CalculationVersion>()
            {
                calcsVersion1,
                calcsVersion2,
                calcsVersion3
            };


            calcsClient
               .GetAllVersionsByCalculationId(calculationId)
               .Returns(new ApiResponse<IEnumerable<CalculationVersion>>(System.Net.HttpStatusCode.OK, calculationVersion));

            ComparePageModel comparePageModel = new ComparePageModel(specsClient, calcsClient, mapper);

            // Act
            IActionResult result = await comparePageModel.OnGet(calculationId);

            // Assert
            result.Should().NotBeNull();

            comparePageModel.Calculation.Should().NotBeNull();

            comparePageModel.Calculation.Description.Should().Be(specCalculation.Description);

            comparePageModel.Calculation.Name.Should().Be(expectedCalculation.Name);

            comparePageModel.Calculations.Select(f => f.Version).Should().BeInDescendingOrder();

            comparePageModel.Calculations.Should().HaveCount(3);

            var firstCalculation = comparePageModel.Calculations.First();

            firstCalculation.Version.Should().Be(calcsVersion3.Version);


        }

    }




}
