using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BurningDowntheHouse.Services
{
	public class OffsetService : IService
	{
		private static readonly string FileName = "offsets.json";
		private static readonly string Url = $"https://raw.githubusercontent.com/LeonBlade/BurningDownTheHouse/master/BurningDownTheHouse/{FileName}";
		private static readonly string LocalOffsetFile = Path.Combine(Environment.CurrentDirectory, FileName);

		public OffsetFile Offsets { get; private set; } = null;

		public async Task Initialize()
		{
			OffsetFile localOffset = null;
			OffsetFile remoteOffset = null;

			// Get the local offset file.
			try
			{
				if (File.Exists(LocalOffsetFile))
				{
					var offsets = JsonConvert.DeserializeObject<OffsetFile>(File.ReadAllText(LocalOffsetFile));
					if (offsets != null)
						localOffset = offsets;
				}
			}
			catch (Exception ex)
			{
				Log.Write(new Exception("Couldn't load offset file", ex), "OffsetService");
			}

			// Fetch latest offsets from online.
			using (var client = new HttpClient())
			{
				try
				{
					var response = await client.GetAsync(Url);
					response.EnsureSuccessStatusCode();
					var body = await response.Content.ReadAsStringAsync();
					remoteOffset = JsonConvert.DeserializeObject<OffsetFile>(body);
				}
				catch (HttpRequestException ex)
				{
					Log.Write(new Exception("Couldn't fetch offsets from online!", ex), "OffsetService");
				}
			}

			if (remoteOffset == null && localOffset == null)
				throw new Exception("Wasn't able to resolve any offsets");

			// If the offsets are the same.
			if (remoteOffset?.GameVersion == localOffset?.GameVersion)
			{
				// Check which offset version is greater and use it.
				if (remoteOffset?.OffsetVersion > localOffset?.OffsetVersion)
					Offsets = remoteOffset;
				else
					Offsets = localOffset;
			}
			else
			{
				// Use the offset with the latest game version.
				if (remoteOffset?.GameVersion.CompareTo(localOffset?.GameVersion) > 0)
					Offsets = remoteOffset;
				else
					Offsets = localOffset;
			}

			if (Offsets == null)
				throw new Exception("Wasn't able to resolve any offsets");
		}

		public Task Start()
		{
			return Task.CompletedTask;
		}

		public Task Dispose()
		{
			return Task.CompletedTask;
		}
	}
}
