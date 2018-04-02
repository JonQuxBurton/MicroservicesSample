using Nancy;

namespace PhoneLineOrderer
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
