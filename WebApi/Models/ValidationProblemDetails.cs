using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using raBudget.Core.Exceptions;

namespace raBudget.WebApi.Models
{
   
    public class ValidationProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "errors")]
        public IDictionary<string, string[]> Errors;
        public ValidationProblemDetails(Core.Exceptions.ValidationException exception)
        {
            this.Title = "Invalid request data";
            this.Status = StatusCodes.Status400BadRequest;
            this.Detail = exception.Message;
            Errors = exception.Failures;
        }

    }
 
}
