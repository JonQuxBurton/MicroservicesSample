using Infrastructure.Rest;
using Moq;
using Polly;
using Polly.NoOp;
using RestSharp;
using Xunit;

namespace Infrastructure.Tests.Rest
{
    public class RestPoster_Post_Should
    {
        [Fact]
        public void Post()
        {
            var expected = new RestResponse();
            var dummyUrl = "url";

            NoOpPolicy noOp = Policy.NoOp();

            var restClientMock = new Mock<IRestClient>();
            restClientMock.Setup(x => x.Execute(It.Is<RestRequest>(y => y.Method == Method.POST && y.Resource == dummyUrl)))
                .Returns(expected);

            var sut = new RestPoster(restClientMock.Object, noOp);

            var actual = sut.Post(dummyUrl, new object());

            Assert.Equal(expected, actual);
        }
    }
}
