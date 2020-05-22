using System;

namespace BurningDownTheHouse
{
	[Serializable]
	public class OffsetFile
	{
		public string GameVersion { get; set; }
		public int OffsetVersion { get; set; }
		public string IsHousingOn { get; set; }
		public string IsItemSelected { get; set; }
		public string PlaceAnywhere { get; set; }
		public string WallPartition { get; set; }
		public string ActiveItem { get; set; }
		public string ActiveItemPosition { get; set; }
		public string MousePosition { get; set; }
	}
}
