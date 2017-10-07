using System;

namespace Infrastructure.DateTimeUtilities
{
    public interface IDateTimeOffsetCreator
    {
        DateTimeOffset Now { get; }
    }
}
