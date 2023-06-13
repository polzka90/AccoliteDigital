using Bogus;
using Core.Application.Contracts.DataTransferObjects;

namespace UnitTest.Application.Fakers;

public static class FakerUserGenerator
{
    public static UserDto SingleUserDtoFake()
    {
        var recordFaker = new Faker<UserDto>();
        return recordFaker.Generate();
    }
}