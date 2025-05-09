using Application.Common.Interfaces;

namespace Application.Services;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}