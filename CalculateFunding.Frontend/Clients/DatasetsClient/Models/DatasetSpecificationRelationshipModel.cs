﻿namespace CalculateFunding.Frontend.Clients.DatasetsClient.Models
{
    using CalculateFunding.Common.Models;

    public class DatasetSpecificationRelationshipModel : Reference
    {
        public SpecificationDataDefinitionRelationshipModel Definition { get; set; }

        public string DatasetName { get; set; }

        public int? Version { get; set; }

        public string DatasetId { get; set; }

        public string RelationshipDescription { get; set; }

        public bool IsProviderData { get; set; }
    }
}
