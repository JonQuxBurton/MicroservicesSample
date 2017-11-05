using Infrastructure.Events;
using Infrastructure.Rest;
using Moq;
using RestSharp;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Events
{
    public class EventGetter_Get_Should
    {
        [Fact]
        public async void ReturnEvents()
        {
            var expected = new RestResponse();
            var dummyUrl = "url";

            var restClientMock = new Mock<IRestClient>();

            var restGetterMock = new Mock<IRestGetter>();
            restGetterMock.Setup(x => x.Get(It.IsAny<RestRequest>()))
                .Returns(Task.FromResult<IRestResponse>(expected));

            var sut = new EventGetter(restGetterMock.Object);

            var actual = await sut.Get(dummyUrl, 0, 100);

            Assert.Equal(expected, actual);
        }
    }
}
