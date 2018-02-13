﻿namespace CalculateFunding.Frontend.Clients.DatasetsClient
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using CalculateFunding.Frontend.Clients;
    using CalculateFunding.Frontend.Clients.CommonModels;
    using CalculateFunding.Frontend.Clients.DatasetsClient.Models;
    using CalculateFunding.Frontend.Helpers;
    using CalculateFunding.Frontend.Interfaces.ApiClient;
    using CalculateFunding.Frontend.Interfaces.Core;
    using CalculateFunding.Frontend.Interfaces.Core.Logging;
    using Microsoft.Extensions.Options;
    using Serilog;

    public class DatasetsApiClient : AbstractApiClient, IDatasetsApiClient
    {
        private string _datasetsPath = "datasets";

        public DatasetsApiClient(
            IOptionsSnapshot<ApiOptions> options,
            IHttpClient httpClient,
            ILogger logger,
            ICorrelationIdProvider correlationIdProvider)
            : base(options, httpClient, logger, correlationIdProvider)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            _datasetsPath = options.Value.DatasetsPath ?? "datasets";
        }

        public async Task<PagedResult<DatasetSearchResultItem>> FindDatasets(SearchFilterRequest filterOptions)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            SearchQueryRequest request = SearchQueryRequest.FromSearchFilterRequest(filterOptions);

            ApiResponse<SearchResults<DatasetSearchResultItem>> results = await PostAsync<SearchResults<DatasetSearchResultItem>, SearchQueryRequest>($"{_datasetsPath}/datasets-search", request);
            if (results.StatusCode == System.Net.HttpStatusCode.OK)
            {
                PagedResult<DatasetSearchResultItem> result = new SearchPagedResult<DatasetSearchResultItem>(filterOptions, results.Content.TotalCount)
                {
                    Items = results.Content.Results,
                    Facets = results.Content.Facets,
                };

                return result;
            }
            else
            {
                return null;
            }

            ////PagedResult<DatasetSearchResultItem> result = new PagedResult<DatasetSearchResultItem>()
            ////{
            ////    TotalPages = 1,
            ////    TotalItems = 3,
            ////    PageSize = 50,
            ////    PageNumber = 1,
            ////};

            ////List<DatasetSearchResultItem> items = new List<DatasetSearchResultItem>
            ////{
            ////    new DatasetSearchResultItem()
            ////    {
            ////        Id = "1",
            ////        LastUpdated = DateTime.Now.AddHours(-1),
            ////        Name = "APT Adjusted Factors Test",
            ////        Status = "Draft",
            ////    },

            ////    new DatasetSearchResultItem()
            ////    {
            ////        Id = "2",
            ////        LastUpdated = DateTime.Now.AddHours(-2),
            ////        Name = "APT Adjusted Factors Test Asif",
            ////        Status = "Published",
            ////    },

            ////    new DatasetSearchResultItem()
            ////    {
            ////        Id = "3",
            ////        LastUpdated = DateTime.Now.AddHours(-3),
            ////        Name = "AP Adjusted Factors New Data",
            ////        Status = "Updated",
            ////    }
            ////};

            ////result.Items = items;
            ////List<SearchFacet> facets = new List<SearchFacet>
            ////{
            ////    new SearchFacet()
            ////    {
            ////        Name = "dataSchema",
            ////        FacetValues = new List<SearchFacetValue>()
            ////    {
            ////        new SearchFacetValue() { Name = "Schema 1", Count = 5 },
            ////        new SearchFacetValue() { Name = "Schema two", Count = 2 }
            ////    }
            ////    },

            ////    new SearchFacet()
            ////    {
            ////        Name = "specification",
            ////        FacetValues = new List<SearchFacetValue>()
            ////    {
            ////        new SearchFacetValue() { Name = "Spec 1", Count = 50 },
            ////        new SearchFacetValue() { Name = "Spec 52", Count = 25 }
            ////    }
            ////    },

            ////    new SearchFacet()
            ////    {
            ////        Name = "periodName",
            ////        FacetValues = new List<SearchFacetValue>()
            ////    {
            ////        new SearchFacetValue() { Name = "2017/18", Count = 2 },
            ////        new SearchFacetValue() { Name = "2018/2019", Count = 1 }
            ////    }
            ////    },

            ////    new SearchFacet()
            ////    {
            ////        Name = "status",
            ////        FacetValues = new List<SearchFacetValue>()
            ////    {
            ////        new SearchFacetValue()
            ////        { Name = "Draft", Count = 1 },
            ////        new SearchFacetValue() { Name = "Published", Count = 1 },
            ////        new SearchFacetValue() { Name = "Updated", Count = 1 }
            ////    }
            ////    }
            ////};

            ////result.Facets = facets;

            ////return Task.FromResult(result);
        }

        public Task<ApiResponse<IEnumerable<DatasetDefinition>>> GetDataDefinitions()
        {
            return GetAsync<IEnumerable<DatasetDefinition>>($"{_datasetsPath}/get-data-definitions");
        }

        public Task<ApiResponse<Reference[]>> GetDefinitions()
        {
            var definitions = new[]
            {
                new Reference("9183", "14/15")
            };

            ApiResponse<Reference[]> response = new ApiResponse<Reference[]>(HttpStatusCode.OK, definitions);

            return Task.FromResult(response);
        }

        public Task<ValidatedApiResponse<CreateNewDatasetResponseModel>> PostDataset(CreateNewDatasetModel dataset)
        {
            Guard.ArgumentNotNull(dataset, nameof(dataset));

            return ValidatedPostAsync<CreateNewDatasetResponseModel, CreateNewDatasetModel>($"{_datasetsPath}/create-new-dataset", dataset);
        }

        public Task<HttpStatusCode> ValidateDataset(ValidateDatasetModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));

            return PostAsync($"{_datasetsPath}/validate-dataset", model);
        }

        public Task<HttpStatusCode> AssignDatasetSchema(AssignDatasetSchemaModel datasetSchema)
        {
            Guard.ArgumentNotNull(datasetSchema, nameof(datasetSchema));

            return PostAsync($"{_datasetsPath}/create-definitionspecification-relationship", datasetSchema);
        }
    }
}
