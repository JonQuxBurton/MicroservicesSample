using Nancy;

namespace FakeBt
{
    public class StatusModule : NancyModule
    {
        public StatusModule() : base("/status")
        {
            Get("", x => {
                return HttpStatusCode.OK;
            });
        }
    }
}
