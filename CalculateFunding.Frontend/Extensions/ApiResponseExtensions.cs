﻿using CalculateFunding.Frontend.Clients.CommonModels;
using CalculateFunding.Frontend.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CalculateFunding.Frontend.Extensions
{
    public static class ApiResponseExtensions
    {
        public static IActionResult IsSuccessOrReturnFailureResult<T>(this ApiResponse<T> apiResponse, string entityName)
        {
            Guard.IsNullOrWhiteSpace(entityName, nameof(apiResponse));

            if (apiResponse == null)
            {
                return new InternalServerErrorResult($"{entityName} API response returned null.");
            }

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new NotFoundObjectResult($"{entityName} not found.");
            }

            if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new InternalServerErrorResult($"{entityName} API call did not return succes, but instead '{apiResponse.StatusCode}'");
            }

            if (EqualityComparer<T>.Default.Equals(apiResponse.Content, default(T)))
            {
                return new NotFoundObjectResult($"{entityName} returned null.");
            }

            return null;
        }
    }
}
