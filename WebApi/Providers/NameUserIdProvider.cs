﻿using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Providers
{
    public class NameUserIdProvider : IUserIdProvider
    {
        #region Methods

        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        #endregion
    }
}