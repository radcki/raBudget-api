using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using raBudget.Core.Dto.Budget;
using raBudget.Core.Dto.User;
using raBudget.Core.Interfaces.Repository;
using raBudget.Domain.Entities;
using raBudget.Domain.Enum;
using Xunit;

namespace raBudget.Core.Tests
{
    public class RegisterUserTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private Mock<IMapper> _mapperMock;

        public RegisterUserTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public void RegisterUser()
        {
            var mockUser = new Mock<User>();
            var mockUserDto = new Mock<UserDto>();
            _repoMock.Setup(x => x.AddAsync(mockUser.Object)).ReturnsAsync(mockUser.Object);

            _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserDto>()))
                       .Returns(mockUser.Object);

            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                       .Returns(mockUserDto.Object);
            /*
            var handler = new RegisterUserHandler(_repoMock.Object, _mapperMock.Object);
            var response = handler.Handle(new CheckInUserRequest(mockUserDto.Object));
            
            Assert.Equal(mockUserDto.Object, response.Result.Data);
            */
        }
    }
}
