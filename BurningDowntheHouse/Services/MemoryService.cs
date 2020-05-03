using Memory;
using System;
using System.Threading.Tasks;

namespace BurningDowntheHouse.Services
{
	public class MemoryService : IService
	{
		public Mem Memory { get; private set; }

		public Task Initialize()
		{
			this.Memory = new Mem();

			if (!this.Memory.OpenProcess("ffxiv_dx11"))
				throw new Exception("Game process couldn't be found");

			return Task.CompletedTask;
		}

		public Task Start()
		{
			return Task.CompletedTask;
		}

		public Task Dispose()
		{
			this.Memory.CloseProcess();
			return Task.CompletedTask;
		}
	}
}
