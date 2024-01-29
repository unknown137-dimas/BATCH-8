using NLog;

class Program
{
	private static readonly Logger logger = LogManager.GetCurrentClassLogger();
	
	static void Main()
	{
		logger.Info("AAAAAAAAAA");
		logger.Info("Program started.");
		logger.Warn("WARN MESSAGE");
		logger.Error("ERROR MESSAGE");
		logger.Fatal("FATAL MESSAGE");
	}
}