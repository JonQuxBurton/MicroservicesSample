using Infrastructure.Rest;
using Polly;
using Xunit;

namespace Infrastructure.Tests.Rest
{
    public class RestPosterFactoryShould
    {
        [Fact]
        public void CreateRestPoster()
        {
            var dummyUrl = "http://url.com";

            var sut = new RestPosterFactory(Policy.NoOp());

            var actual = sut.Create(dummyUrl);

            Assert.IsType<RestPoster>(actual);
        }
    }
}
