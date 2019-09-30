using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;

namespace raBudget.Core.Handlers.UserHandlers.SetDefaultBudget
{
    public class SetDefaultBudgetHandler : IRequestHandler<SetDefaultBudgetRequest, Unit>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IAuthenticationProvider _authenticationProvider;

        public SetDefaultBudgetHandler(IBudgetRepository budgetRepository, IUserRepository userRepository, IMapper mapper, IAuthenticationProvider authenticationProvider)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _authenticationProvider = authenticationProvider;
        }

        public async Task<Unit> Handle(SetDefaultBudgetRequest request, CancellationToken cancellationToken)
        {
            var userEntity = await _userRepository.GetByIdAsync(_authenticationProvider.User.UserId);
            userEntity.DefaultBudgetId = request.BudgetId;
            await _userRepository.UpdateAsync(userEntity);
            await _userRepository.SaveChangesAsync(cancellationToken);
            return new Unit();
        }
    }
}