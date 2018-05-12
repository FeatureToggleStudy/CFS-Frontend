﻿namespace CalculateFunding.Frontend.Pages.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using CalculateFunding.Frontend.Clients.CommonModels;
    using CalculateFunding.Frontend.Clients.SpecsClient.Models;
    using CalculateFunding.Frontend.Extensions;
    using CalculateFunding.Frontend.Helpers;
    using CalculateFunding.Frontend.Interfaces.ApiClient;
    using CalculateFunding.Frontend.Properties;
    using CalculateFunding.Frontend.ViewModels.Specs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateSpecificationPageModel : PageModel
    {
        private readonly ISpecsApiClient _specsClient;

        private readonly IMapper _mapper;

        public CreateSpecificationPageModel(ISpecsApiClient specsClient, IMapper mapper)
        {
            Guard.ArgumentNotNull(specsClient, nameof(specsClient));
            Guard.ArgumentNotNull(mapper, nameof(mapper));

            _specsClient = specsClient;
            _mapper = mapper;
        }

        public IEnumerable<SelectListItem> FundingStreams { get; set; }

        public IEnumerable<SelectListItem> FundingPeriods { get; set; }

        public string FundingPeriodId { get; set; }

        [BindProperty]
        public CreateSpecificationViewModel CreateSpecificationViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(string fundingPeriodId = null)
        {
            if(!string.IsNullOrWhiteSpace(fundingPeriodId))
            {
                FundingPeriodId = fundingPeriodId;
            }

            await TaskHelper.WhenAllAndThrow(PopulateFundingPeriods(fundingPeriodId), PopulateFundingStreams());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string fundingPeriodId = null)
        {
            if (!string.IsNullOrWhiteSpace(CreateSpecificationViewModel.Name))
            {
                ApiResponse<Specification> existingSpecificationResponse = await this._specsClient.GetSpecificationByName(CreateSpecificationViewModel.Name);

                if (existingSpecificationResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    this.ModelState.AddModelError($"{nameof(CreateSpecificationViewModel)}.{nameof(CreateSpecificationViewModel.Name)}", ValidationMessages.SpecificationAlreadyExists);
                }
            }

            if (!ModelState.IsValid)
            {
                await TaskHelper.WhenAllAndThrow(PopulateFundingPeriods(fundingPeriodId), PopulateFundingStreams());
                return Page();
            }

            CreateSpecificationModel specification = _mapper.Map<CreateSpecificationModel>(CreateSpecificationViewModel);

            await _specsClient.PostSpecification(specification);

            return Redirect($"/specs?fundingPeriodId={specification.FundingPeriodId}");
        }

        private async Task PopulateFundingStreams()
        {
            var fundingStreamsResponse = await _specsClient.GetFundingStreams();

            if (fundingStreamsResponse.StatusCode == HttpStatusCode.OK && !fundingStreamsResponse.Content.IsNullOrEmpty())
            {
                var fundingStreams = fundingStreamsResponse.Content;

                FundingStreams = fundingStreams.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = m.Name
                }).ToList();
            }
            else
            {
                throw new InvalidOperationException($"Unable to retreive Funding Streams. Status Code = {fundingStreamsResponse.StatusCode}");
            }
        }

        private async Task PopulateFundingPeriods(string fundingPeriodId)
        {
            ApiResponse<IEnumerable<Reference>> fundingPeriodsResponse = await _specsClient.GetFundingPeriods();

            if (fundingPeriodsResponse.StatusCode == HttpStatusCode.OK && !fundingPeriodsResponse.Content.IsNullOrEmpty())
            {
                var fundingPeriods = fundingPeriodsResponse.Content;

                FundingPeriods = fundingPeriods.Select(m => new SelectListItem
                {
                    Value = m.Id,
                    Text = m.Name,
                    Selected = m.Id == fundingPeriodId
                }).ToList();
            }
            else
            {
                throw new InvalidOperationException($"Unable to retreive Funding Streams. Status Code = {fundingPeriodsResponse.StatusCode}");
            }
        }
    }
}