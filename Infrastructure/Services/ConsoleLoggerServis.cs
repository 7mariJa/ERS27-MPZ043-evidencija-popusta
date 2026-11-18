using Application.Interfaces.Services;

namespace Infrastructure.Services;

// Drugi logger - poruke ispisuje u konzolu umjesto u datoteku
public sealed class ConsoleLoggerServis : ILoggerServis
{
    public bool Loguj(string poruka)
    {
        System.Console.WriteLine("[LOG " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "]: " + poruka);
        return true;
    }
}
