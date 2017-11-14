﻿using Newtonsoft.Json;

namespace Allocations.Web.ApiClient.Models
{

    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("featureFile")]
        public string FeatureFile { get; set; }
        [JsonProperty("calculation")]
        public ProductCalculation Calculation { get; set; }
        [JsonProperty("testProviders")]
        public Reference[] TestProviders { get; set; }
    }

    //[] Move to budget/folder??
}