using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Core.Dto.Base;

namespace raBudget.Core.Dto.User.Response
{
    /// <summary>
    /// Persist logged user
    /// </summary>
    /// <returns>UserId in form of Guid</returns>
    public class AddUserResponse : BaseResponse<Guid>
    {
    }
}
