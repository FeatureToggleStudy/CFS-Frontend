﻿namespace CalculateFunding.Frontend.ViewModels.Results
{
    using System;
    using CalculateFunding.Frontend.ViewModels.Common;
    using Newtonsoft.Json;

    public class ProviderSearchResultItemViewModel : ReferenceViewModel
    {
        public string Upin { get; set; }

        public string Ukprn { get; set; }

        public string Urn { get; set; }

        public string EstablishmentNumber { get; set; }

        public string ProviderType { get; set; }

        public string ProviderSubType { get; set; }

        public string LocalAuthority { get; set; }

        public string ProviderId { get; set; }

        [JsonProperty("openDate")]
        public DateTime? DateOpened { get; set; }

        public DateTime ConvertDate { get; set; }

        public DateTime LocalAuthorityChangeDate { get; set; }

        public string PreviousLocalAuthority { get; set; }

        [JsonProperty("closeDate")]
        public DateTime? DateClosed { get; set; }

        public string DateOpenedDisplay
        {
            get
            {
                return DateOpened.HasValue ? DateOpened.Value.ToString("dd/MM/yyyy") : "Unknown";
            }
        }
    }
}
