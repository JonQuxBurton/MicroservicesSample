using System;

namespace Infrastructure.DateTimeUtilities
{
    public class DateTimeOffsetCreator : IDateTimeOffsetCreator
    {
        public DateTimeOffset Now => new DateTimeOffset(DateTime.Now);
    }
}
