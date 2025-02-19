﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CalculateFunding.Common.Utility;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Frontend.Clients.ResultsClient.Models;
using CalculateFunding.Frontend.Clients.ResultsClient.Models.Results;
using CalculateFunding.Frontend.Interfaces.ApiClient;
using CalculateFunding.Frontend.ViewModels.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Frontend.Pages.Results
{
    public abstract class ProviderResultsBasePageModel : PageModel
    {
        private readonly IResultsApiClient _resultsApiClient;
        private readonly IMapper _mapper;
        private readonly ISpecsApiClient _specsApiClient;
        private readonly ILogger _logger;

        public ProviderResultsBasePageModel(IResultsApiClient resultsApiClient, IMapper mapper, ISpecsApiClient specsApiClient, ILogger logger)
        {
            _resultsApiClient = resultsApiClient;
            _mapper = mapper;
            _specsApiClient = specsApiClient;
            _logger = logger;
        }

        public ProviderResultsViewModel ViewModel { get; set; }

        public IEnumerable<SelectListItem> FundingPeriods { get; set; }

        public IEnumerable<SelectListItem> Specifications { get; set; }

        [BindProperty]
        public string FundingPeriodId { get; set; }

        public string SpecificationId { get; set; }

        public string ProviderId { get; set; }

        public async Task<IActionResult> OnGetAsync(string providerId, string fundingPeriodId = null, string specificationId = null)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            await PopulateAsync(providerId, fundingPeriodId, specificationId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string providerId, string fundingPeriodId = null, string specificationId = null)
        {
            return await OnGetAsync(providerId, fundingPeriodId, specificationId);
        }

        async Task PopulateAsync(string providerId, string fundingPeriodId = null, string specificationId = null)
        {
            await PopulatePeriods(fundingPeriodId);

            if (string.IsNullOrWhiteSpace(fundingPeriodId))
            {
                fundingPeriodId = FundingPeriods?.First().Value;
            }

            FundingPeriodId = fundingPeriodId;

            await PopulateSpecifications(providerId, specificationId);

            ProviderId = providerId;

            SpecificationId = specificationId;

            ApiResponse<Provider> apiResponse = await _resultsApiClient.GetProviderByProviderId(providerId);

            if (apiResponse.StatusCode != HttpStatusCode.OK && apiResponse.Content == null)
            {
                throw new InvalidOperationException($"Unable to retreive Provider information: Status Code = {apiResponse.StatusCode}");
            }

            Provider response = apiResponse.Content;

            ProviderResultsViewModel viewModel = new ProviderResultsViewModel
            {
                ProviderName = response.Name,
                ProviderType = response.ProviderType,
                ProviderSubtype = response.ProviderSubtype,
                LocalAuthority = response.LocalAuthority,
                Upin = response.UPIN,
                Ukprn = response.UKPRN,
                Urn = response.URN,
                DateOpened = response.DateOpened.HasValue ? response.DateOpened.Value.ToString("dd/MM/yyyy") : "Unknown"
            };

            ViewModel = viewModel;

            if (!string.IsNullOrWhiteSpace(specificationId))
            {
                ApiResponse<ProviderResults> providerResponse = await _resultsApiClient.GetProviderResults(providerId, specificationId);

                if (providerResponse.StatusCode == HttpStatusCode.OK && providerResponse.Content != null)
                {
                    PopulateResults(providerResponse);
                }
                else
                {
                    _logger.Warning("There were no providers for the given specification Id " + specificationId);
                }
            }
        }

        public abstract void PopulateResults(ApiResponse<ProviderResults> providerResponse);

        private async Task PopulatePeriods(string fundingPeriodId = null)
        {
            ApiResponse<IEnumerable<Reference>> periodsResponse = await _specsApiClient.GetFundingPeriods();

            if (periodsResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Unable to retreive Periods: Status Code = {periodsResponse.StatusCode}");
            }
            IEnumerable<Reference> fundingPeriods = periodsResponse.Content;

            if (string.IsNullOrWhiteSpace(fundingPeriodId))
            {
                fundingPeriodId = FundingPeriodId;
            }

            FundingPeriods = fundingPeriods.Select(m => new SelectListItem
            {
                Value = m.Id,
                Text = m.Name,
                Selected = m.Id == fundingPeriodId
            }).ToList();
        }

        private async Task PopulateSpecifications(string providerId, string specificationId = null)
        {
            ApiResponse<IEnumerable<string>> specResponse = await _resultsApiClient.GetSpecificationIdsForProvider(providerId);

            if (specResponse.Content != null && specResponse.StatusCode == HttpStatusCode.OK)
            {
                IEnumerable<string> specificationIds = specResponse.Content;
                if (string.IsNullOrWhiteSpace(specificationId))
                {
                    specificationId = SpecificationId;
                }

                Dictionary<string, Clients.SpecsClient.Models.SpecificationSummary> specificationSummaries = new Dictionary<string, Clients.SpecsClient.Models.SpecificationSummary>();

                if (specificationIds.Any())
                {
                    ApiResponse<IEnumerable<Clients.SpecsClient.Models.SpecificationSummary>> specificationSummaryLookup = await _specsApiClient.GetSpecificationSummaries(specificationIds);
                    if (specificationSummaryLookup == null)
                    {
                        throw new InvalidOperationException("Specification Summary Lookup returned null");
                    }

                    if (specificationSummaryLookup.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidOperationException($"Specification Summary lookup returned HTTP Status code {specificationSummaryLookup.StatusCode}");
                    }

                    if (!specificationSummaryLookup.Content.IsNullOrEmpty())
                    {
                        foreach (Clients.SpecsClient.Models.SpecificationSummary specSummary in specificationSummaryLookup.Content)
                        {
                            specificationSummaries.Add(specSummary.Id, specSummary);
                        }
                    }
                }

                List<SelectListItem> selectListItems = new List<SelectListItem>();

                foreach (string specId in specificationIds)
                {
                    string specName = specId;

                    if (specificationSummaries.ContainsKey(specId))
                    {
                        if (specificationSummaries[specId].FundingPeriod.Id != FundingPeriodId)
                        {
                            continue;
                        }

                        specName = specificationSummaries[specId].Name;
                    }
                    else
                    {
                        continue;
                    }

                    selectListItems.Add(new SelectListItem
                    {
                        Value = specId,
                        Text = specName,
                        Selected = specId == specificationId,
                    });
                }

                Specifications = selectListItems.OrderBy(o => o.Text);
            }
            else
            {
                throw new InvalidOperationException($"Unable to retrieve provider result Specifications: Status Code = {specResponse.StatusCode}");
            }

        }
    }
}
