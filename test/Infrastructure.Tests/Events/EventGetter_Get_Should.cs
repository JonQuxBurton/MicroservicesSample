using Infrastructure.Events;
using Moq;
using RestSharp;
using Xunit;

namespace Infrastructure.Tests.Events
{
    public class EventGetter_Get_Should
    {
        [Fact]
        public void ReturnEvents()
        {
            var expected = new RestResponse();
            var dummyUrl = "url";

            var restClientMock = new Mock<IRestClient>();
            restClientMock
                .Setup(x => x.Execute(It.Is<RestRequest>(y => y.Resource == dummyUrl && y.Method == Method.GET)))
                .Returns(expected);

            var sut = new EventGetter(restClientMock.Object);

            var actual = sut.Get(dummyUrl, 0, 100);

            Assert.Equal(expected, actual);
        }
    }
}
