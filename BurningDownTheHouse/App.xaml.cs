using BurningDownTheHouse.Services;
using BurningDownTheHouse.Views;
using ConceptMatrix;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BurningDownTheHouse
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static readonly ServiceManager Services = new ServiceManager();

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			this.Dispatcher.UnhandledException += this.Dispatcher_UnhandledException;
			Current.DispatcherUnhandledException += this.Current_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
			Log.OnException += this.OnException;

			base.OnStartup(e);

			Task.Run(this.Start);

			Log.Write("Starting up...");
		}

		private void OnException(ExceptionDispatchInfo ex, Log.Severity severity, string category)
		{
			ErrorDialog.ShowError(ex, severity == Log.Severity.Critical);
		}

		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			Log.Write(e.Exception, @"Unhandled Task", Log.Severity.Critical);
		}

		private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Log.Write(e.Exception, @"Unhandled Current Dispatcher", Log.Severity.Critical);
		}

		private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Log.Write(e.Exception, @"Unhandled Dispatcher", Log.Severity.Critical);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Write(e.ExceptionObject as Exception, @"Unhandled", Log.Severity.Critical);
		}

		private async Task Start()
		{
			try
			{
				await Services.InitializeServices();

				Current.Dispatcher.Invoke(() =>
				{
					this.MainWindow = new MainWindow();
					this.MainWindow.Show();
				});
			}

			catch (Exception ex)
			{
				Log.Write(ex);
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			var t = Services.ShutdownServices();
			t.Wait();

			base.OnExit(e);

			Log.Write("Shutting down...");
		}
	}
}
