﻿using CalculateFunding.Frontend.Clients.CommonModels;
using CalculateFunding.Frontend.Clients.SpecsClient.Models;
using CalculateFunding.Frontend.Interfaces.ApiClient;
using CalculateFunding.Frontend.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Frontend.Controllers
{
    public class SpecificationController : Controller
    {
        private readonly ISpecsApiClient _specsClient;

        public SpecificationController(ISpecsApiClient specsApiClient)
        {
            _specsClient = specsApiClient;
        }

        [Route("api/specifications-by-period/{periodId}")]
        public async Task<IActionResult> GetSpecificationsByFundingPeriod(string fundingPeriodId)
        {
            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse = await _specsClient.GetSpecifications(fundingPeriodId);
            if (apiResponse == null)
            {
                return new StatusCodeResult(500);
            }

            if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new StatusCodeResult((int)apiResponse.StatusCode);
            }

            if(apiResponse.Content == null)
            {
                return new ObjectResult("List of Specifications was null")
                {
                    StatusCode = 500,
                };
            }

            List<ReferenceViewModel> result = new List<ReferenceViewModel>();

            foreach (SpecificationSummary specification in apiResponse.Content.OrderBy(o => o.Name))
            {
                result.Add(new ReferenceViewModel(specification.Id, specification.Name));
            }

            return Ok(result);
        }
    }
}
