using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;

namespace raBudget.Core.Features.User.Command
{
    public class SetDefaultBudget
    {
        public class Command : IRequest
        {
            public int BudgetId;

            public Command(int budgetId)
            {
                BudgetId = budgetId;
            }
        }


        public class Validator : AbstractValidator<Command>
        {
            public Validator(IBudgetRepository budgetRepository, IAuthenticationProvider authenticationProvider)
            {
                RuleFor(x => x.BudgetId).NotEmpty();

                /* Check if user has access to budget */
                var availableBudgets = budgetRepository.ListAvailableBudgets(authenticationProvider.User.UserId).Result;
                RuleFor(x => availableBudgets.Any(s => s.Id == x.BudgetId))
                   .NotEqual(false)
                   .WithMessage("Requested budget does not exist.");
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IBudgetRepository _budgetRepository;
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly IAuthenticationProvider _authenticationProvider;

            public Handler(IBudgetRepository budgetRepository, IUserRepository userRepository, IMapper mapper, IAuthenticationProvider authenticationProvider)
            {
                _budgetRepository = budgetRepository;
                _userRepository = userRepository;
                _mapper = mapper;
                _authenticationProvider = authenticationProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var userEntity = await _userRepository.GetByIdAsync(_authenticationProvider.User.UserId);
                userEntity.DefaultBudgetId = request.BudgetId;
                await _userRepository.UpdateAsync(userEntity);
                await _userRepository.SaveChangesAsync(cancellationToken);
                return new Unit();
            }
        }
    }
}