using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Handlers.Budget.Query;
using raBudget.Core.Interfaces;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using Xunit;

namespace raBudget.Core.Tests.Handlers.Budget
{
    public class ListAvailableBudgetsFixture : IDisposable
    {
        public readonly Mock<IBudgetRepository> RepoMock;
        public readonly Mock<IMapper> MapperMock;
        public readonly Mock<IAuthenticationProvider> AuthenticationProviderMock;

        public List<Domain.Entities.Budget> SampleBudgetEntities;
        public List<BudgetDto> SampleBudgetDtoEntities;

        public ListAvailableBudgets.Handler RequestHandler;
        public Task<ListAvailableBudgets.Response> RequestResponse;

        public ListAvailableBudgetsFixture()
        {
            RepoMock = new Mock<IBudgetRepository>();
            MapperMock = new Mock<IMapper>();
            AuthenticationProviderMock = new Mock<IAuthenticationProvider>();

            SampleBudgetEntities = new List<Domain.Entities.Budget>()
                                   {
                                       new Domain.Entities.Budget()
                                       {
                                           Id = It.IsAny<int>(),
                                           Name = It.IsAny<string>(),
                                           CurrencyCode = It.IsAny<eCurrency>(),
                                           StartingDate = It.IsAny<DateTime>()
                                       }
                                   };

            SampleBudgetDtoEntities = new List<BudgetDto>()
                                      {
                                          new BudgetDto()
                                          {
                                              BudgetId = It.IsAny<int>(),
                                              Name = It.IsAny<string>(),
                                              Currency = It.IsAny<Currency>(),
                                              StartingDate = It.IsAny<DateTime>()
                                          }
                                      };

            var mockUser = new Mock<User>();
            var mockUserDto = new Mock<UserDto>();
            RepoMock.Setup(x => x.ListAvailableBudgets(It.IsAny<Guid>()))
                    .ReturnsAsync(SampleBudgetEntities);

            MapperMock.Setup(m => m.Map<User>(It.IsAny<UserDto>()))
                      .Returns(mockUser.Object);
            MapperMock.Setup(m => m.Map<IEnumerable<BudgetDto>>(It.IsAny<IEnumerable<Domain.Entities.Budget>>()))
                      .Returns(SampleBudgetDtoEntities);

            AuthenticationProviderMock.Setup(m => m.User).Returns(mockUserDto.Object);
            AuthenticationProviderMock.Setup(m => m.IsAuthenticated).Returns(true);

            RequestHandler = new ListAvailableBudgets.Handler(RepoMock.Object, MapperMock.Object, AuthenticationProviderMock.Object);
            RequestResponse = RequestHandler.Handle(new ListAvailableBudgets.Query(), new CancellationToken());
        }

        public void Dispose()
        {
        }
    }

    public class ListUserBudgetsTests : IClassFixture<ListAvailableBudgetsFixture>
    {
        readonly ListAvailableBudgetsFixture fixture;

        public ListUserBudgetsTests(ListAvailableBudgetsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanReceiveBudgetsList()
        {
            //Assert.Equal(eResponseType.Success, fixture.RequestResponse.Result.ResponseType);
        }

        [Fact]
        public void ReturnsAllData()
        {
            //Assert.Equal(fixture.SampleBudgetEntities.Count, fixture.RequestResponse.Result.Data.Count());
        }

        [Fact]
        public void ReturnsDtos()
        {
            //Assert.Equal(fixture.SampleBudgetDtoEntities, fixture.RequestResponse.Result.Data);
        }

        [Fact]
        public void DoesNotReturnEntities()
        {
            /* foreach (var data in fixture.RequestResponse)
             {
                 Assert.False(data.GetType().IsSubclassOf(typeof(BaseEntity<Domain.Entities.Budget>)));
             }*/
        }
    }
}