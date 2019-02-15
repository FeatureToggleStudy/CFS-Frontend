﻿using System;

namespace CalculateFunding.Frontend.Clients.DatasetsClient.Models
{
	public class DatasetVersionSearchResultModel
	{
		public string Id { get; set; }

		public string DatasetId { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string ChangeNote { get; set; }

		public int Version { get; set; }

		public string DefinitionName { get; set; }

		public DateTimeOffset LastUpdatedDate { get; set; }

		public string LastUpdatedByName { get; set; }

		public string BlobName { get; set; }
	}
}
