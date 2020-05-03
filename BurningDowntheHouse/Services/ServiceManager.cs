using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BurningDowntheHouse.Services
{
	public class ServiceManager
	{
		private List<IService> services = new List<IService>();

		public delegate void ServiceEvent(string serviceName);

		public static event ServiceEvent OnServiceInitializing;
		public static event ServiceEvent OnServiceStarting;

		public bool IsInitialized { get; private set; } = false;
		public bool IsStarted { get; private set; } = false;

		public T Get<T>()
			where T : IService
		{
			foreach (var service in this.services)
			{
				if (typeof(T).IsAssignableFrom(service.GetType()))
					return (T)service;
			}

			throw new Exception($"Failed to find services: {typeof(T)}");
		}

		public async Task Add<T>()
			where T : IService, new()
		{
			try
			{
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
			}
			catch (Exception ex)
			{
				Log.Write(new Exception($"Failed to initialize service: {typeof(T).Name}", ex));
			}
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
				await service.Dispose();
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
