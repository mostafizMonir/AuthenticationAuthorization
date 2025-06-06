namespace DotNetChannel.Models;

public class IdPrinter(IdGenerator idGenerator)
{
    public Guid PrintId()
    {
        return idGenerator.Id;
    }

}

