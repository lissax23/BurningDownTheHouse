using BurningDowntheHouse.Services;
using System.Threading.Tasks;
using System.Windows;

namespace BurningDowntheHouse
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static readonly ServiceManager Services = new ServiceManager();

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Task.Run(this.Start);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			var task = Services.ShutdownServices();
			task.Wait();

			base.OnExit(e);

			Log.Write("Shutdown");
		}

		private async void Start()
		{
			await Services.Add<OffsetService>();
			await Services.Add<MemoryService>();

			await Services.StartServices();
		}
	}
}
