﻿using System;

namespace CalculateFunding.Frontend.Clients.SpecsClient.Models
{
    public class SpecificationCalculationExecutionStatusModel
    {
        public string SpecificationId { get; set; }

        public int PercentageCompleted { get; set; }

        public CalculationProgressStatus CalculationProgress { get; set; }

        public string ErrorMessage { get; set; }

        public DateTimeOffset? PublishedResultsRefreshedAt { get; set; }

        public int NewCount { get; set; }

        public int ApprovedCount { get; set; }

        public int UpdatedCount { get; set; }

        public int PublishedCount { get; set; }

        public bool HasChanges
        {
            get
            {
                return (NewCount + ApprovedCount + UpdatedCount + PublishedCount) > 0;
            }
        }
    }
}
