using Presentation.Composition;
using Presentation.Console;

// prvi argument: in-memory ili json, drugi argument: datoteka ili konzola
string rezim = args.Length > 0 ? args[0] : "in-memory";
string logger = args.Length > 1 ? args[1] : "datoteka";
AppContainer container = AppContainer.Create(rezim, logger);
new ConsoleMenu(container.Io, container).Run();
