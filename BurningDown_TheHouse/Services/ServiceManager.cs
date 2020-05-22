using BurningDownTheHouse.Services;
using ConceptMatrix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BurningDownTheHouse.Services
{
	public class ServiceManager : IServices
	{
		private readonly List<IService> services = new List<IService>();

		public ServiceManager()
		{
			ConceptMatrix.Services.RegisterServicesProvider(this);
		}

		public delegate void ServiceEvent(string serviceName);

		public static event ServiceEvent OnServiceInitializing;
		public static event ServiceEvent OnServiceStarting;

		public bool IsInitialized { get; private set; } = false;
		public bool IsStarted { get; private set; } = false;

		public async Task Add<T>() where T : IService, new()
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();

				var serviceName = GetServiceName<T>();

				var service = Activator.CreateInstance<T>();
				this.services.Add(service);

				OnServiceInitializing?.Invoke(serviceName);
				await service.Initialize();

				if (this.IsStarted)
				{
					OnServiceStarting?.Invoke(serviceName);
					await service.Start();
				}

				Log.Write($"Added service: {serviceName} in {sw.ElapsedMilliseconds}ms", "Services");
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to initialize service: {typeof(T).Name}", ex));
			}
		}

		public T Get<T>() where T : IService
		{
			foreach (var service in this.services)
				if (typeof(T).IsAssignableFrom(service.GetType()))
					return (T)service;

			throw new Exception($"Failed to find service: {typeof(T)}");
		}

		public async Task InitializeServices()
		{
			await this.Add<InjectionService>();
			await this.Add<OffsetsService>();
			await this.Add<SelectedItemService>();

			this.IsInitialized = true;
			Log.Write("Services Initialized", "Services");

			await this.StartServices();
		}

		public async Task StartServices()
		{
			var services = new List<IService>(this.services);
			services.Reverse();
			foreach (var service in services)
			{
				OnServiceStarting?.Invoke(GetServiceName(service.GetType()));
				await service.Start();
			}
		}

		public async Task ShutdownServices()
		{
			foreach (var service in this.services)
				await service.Shutdown();
		}

		private static string GetServiceName<T>()
		{
			return GetServiceName(typeof(T));
		}

		private static string GetServiceName(Type type)
		{
			return type.Name;
		}
	}
}
