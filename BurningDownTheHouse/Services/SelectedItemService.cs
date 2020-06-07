using Anamnesis;
using Anamnesis.Offsets;
using ConceptMatrix;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BurningDownTheHouse.Services
{
	public class SelectedItemService : IService
	{
		private bool IsAlive = false;

		private IMemory<bool> housingOnMem;
		private IMemory<byte> itemSelectedMem;

		private ulong lastItemAddress = 69;

		public delegate void SelectedItemEvent(IMemory<Vector> itemPosition);

		public event SelectedItemEvent SelectionChanged;

		public Task Initialize()
		{
			this.IsAlive = true;
			return Task.CompletedTask;
		}

		public Task Start()
		{
			var offsets = App.Services.Get<OffsetsService>();

			this.housingOnMem = new BaseOffset<bool>(offsets.Get("IsHousingOn")).GetMemory();
			this.itemSelectedMem = new BaseOffset<byte>(offsets.Get("IsItemSelected")).GetMemory();

			Task.Run(this.Watch);

			return Task.CompletedTask;
		}

		private async Task Watch()
		{
			await Task.Delay(500);

			var offsets = App.Services.Get<OffsetsService>();
			var type = typeof(MemoryBase);
			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var finfo = type.GetField("address", flags);

			while (this.IsAlive)
			{
				await Task.Delay(32);

				// Housing is on and item is selected
				if (this.housingOnMem.Value)
				{
					var baseAddr = new BaseOffset(offsets.Get("ActiveItem"));
					var position = baseAddr.GetMemory(new Offset<Vector>(offsets.Get("ActiveItemPosition")));

					var address = ((UIntPtr)finfo.GetValue(position)).ToUInt64();

					if (address != 0 && address != this.lastItemAddress)
						SelectionChanged?.Invoke(position);

					this.lastItemAddress = address;
				}
			}
		}

		public Task Shutdown()
		{
			this.IsAlive = false;
			return Task.CompletedTask;
		}
	}
}
