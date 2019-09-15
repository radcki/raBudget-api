using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Handlers.BudgetHandlers.GetBudget;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using Xunit;

namespace raBudget.Core.Tests.Handlers.Budget
{
    public class GetBudgetFixture : IDisposable
    {
        public readonly Mock<IBudgetRepository> RepoMock;
        public readonly Mock<IMapper> MapperMock;
        public readonly Mock<IAuthenticationProvider> AuthenticationProviderMock;

        public Domain.Entities.Budget RepositoryResult;
        public BudgetDetailsDto MapperResult;

        public GetBudgetHandler RequestHandler;
        public Task<GetBudgetResponse> RequestResponse;
        public Task<GetBudgetResponse> IncorrectRequestResponse;

        public GetBudgetFixture()
        {
            RepoMock = new Mock<IBudgetRepository>();
            MapperMock = new Mock<IMapper>();
            AuthenticationProviderMock = new Mock<IAuthenticationProvider>();

            RepositoryResult = new Domain.Entities.Budget()
                               {
                                   Id = It.IsAny<int>(),
                                   Name = It.IsAny<string>(),
                                   CurrencyCode = It.IsAny<eCurrency>(),
                                   StartingDate = It.IsAny<DateTime>()
                               };

            MapperResult = new BudgetDetailsDto()
                           {
                               BudgetId = It.IsAny<int>(),
                               Name = It.IsAny<string>(),
                               Currency = It.IsAny<string>(),
                               StartingDate = It.IsAny<DateTime>()
                           };

            var mockUser = new Mock<User>();
            var mockUserDto = new Mock<UserDto>();

            RepoMock.Setup(x => x.GetByIdAsync(It.IsInRange(int.MinValue, 0, Range.Inclusive)))
                    .ReturnsAsync(default(Domain.Entities.Budget));

            RepoMock.Setup(x => x.GetByIdAsync(It.IsInRange(1,int.MaxValue,Range.Inclusive)))
                    .ReturnsAsync(RepositoryResult);

            MapperMock.Setup(m => m.Map<BudgetDetailsDto>(It.IsAny<Domain.Entities.Budget>()))
                      .Returns(MapperResult);

            AuthenticationProviderMock.Setup(m => m.User).Returns(mockUserDto.Object);
            AuthenticationProviderMock.Setup(m => m.IsAuthenticated).Returns(true);

            RequestHandler = new GetBudgetHandler(RepoMock.Object, MapperMock.Object, AuthenticationProviderMock.Object);
            RequestResponse = RequestHandler.Handle(new GetBudgetRequest(1), new CancellationToken());
            IncorrectRequestResponse = RequestHandler.Handle(new GetBudgetRequest(0), new CancellationToken());
        }

        public void Dispose()
        {
        }
    }

    public class GetBudgetTests : IClassFixture<GetBudgetFixture>
    {
        readonly GetBudgetFixture _fixture;

        public GetBudgetTests(GetBudgetFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void ReturnsSuccessFromGoodQuery()
        {
            Assert.Equal(eResponseType.Success, _fixture.RequestResponse.Result.ResponseType);
        }

        [Fact]
        public void ReturnsNotFoundFromIncorrectQuery()
        {
            Assert.Equal(eResponseType.NoDataFound, _fixture.IncorrectRequestResponse.Result.ResponseType);
        }

        [Fact]
        public void ReturnsDtos()
        {
            Assert.Equal(_fixture.MapperResult, _fixture.RequestResponse.Result.Data);
        }

        [Fact]
        public void DoesNotReturnEntities()
        {
            Assert.False(_fixture.RequestResponse.Result.Data.GetType().IsSubclassOf(typeof(BaseEntity<Domain.Entities.Budget>)));
        }
    }
}