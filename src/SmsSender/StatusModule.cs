using Nancy;

namespace SmsSender
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
