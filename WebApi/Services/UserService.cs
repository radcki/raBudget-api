namespace WebApi.Services
{
    public class UserService
    {
        /*
        private readonly DataContext DatabaseContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;


        public UserService(DataContext context, ILoggerFactory loggerFactory)
        {
            DatabaseContext = context;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("UserService");
        }

        public BaseResult<User> GetById(Guid userId)
        {
            var user = DatabaseContext.Users.Find(userId);
            return new BaseResult<User>()
                   {
                       Data = user,
                       Result = user != null ? eResultType.Success : eResultType.NoDataFound
                   };
        }

        public BaseResult<User> GetByClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.UserId();
            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            
            if (userId.IsNullOrDefault())
            {
                return new BaseResult<User>(){Result = eResultType.NoDataFound};
            }

            var user =  GetById(userId.Value).Data;

            
            if (user == null)
            {
                user = new User()
                       {
                           Id = userId.Value,
                           CreationTime = DateTime.Now,
                           Email = email
                       };
                DatabaseContext.Users.Add(user);
            }

            return new BaseResult<User>()
                   {
                       Data = user,
                       Result  = eResultType.Success
            };
        }

        public BaseResult DeleteUser(Guid userId)
        {
            var userEntity = GetById(userId).Data;
            if (userEntity == null)
            {
                return new BaseResult() { Result = eResultType.NoDataFound };
            }
            var userBudgets = DatabaseContext.Budgets.Where(x => x.Id == userId);

            DatabaseContext.Budgets.RemoveRange(userBudgets);
            DatabaseContext.Users.Remove(userEntity);
            try
            {
                DatabaseContext.SaveChanges();
                return new BaseResult(){Result = eResultType.Success};
            }
            catch (Exception e)
            {
                _logger.LogError("DeleteUser exception",e);
                return new BaseResult() { Result = eResultType.Error };
            }
        }
        */
    }
}