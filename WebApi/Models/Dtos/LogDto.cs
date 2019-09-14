using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebApi.Models.Dtos
{
    public class LogDto
    {
        #region Properties

        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        #endregion
    }
}