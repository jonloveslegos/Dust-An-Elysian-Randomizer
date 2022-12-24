using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Dust
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			string text = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), inherit: false).GetValue(0)).Value.ToString();
			Mutex mutex = new Mutex(initiallyOwned: false, "Global\\" + text);
			try
			{
				if (!mutex.WaitOne(0, exitContext: false))
				{
					return;
				}
			}
			catch (AbandonedMutexException)
			{
			}
			using (Game1 game = new Game1())
			{
				game.Run();
			}
			mutex.ReleaseMutex();
		}
	}
}
