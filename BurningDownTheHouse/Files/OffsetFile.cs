using Anamnesis.Offsets;
using System;

namespace BurningDownTheHouse.Files
{
	[Serializable]
	public class OffsetFile
	{
		public static OffsetFile Main;

		public string GameVersion { get; set; }
		public int OffsetVersion { get; set; }
		public Offset<bool> IsHousingOn { get; set; }
		public Offset<bool> IsItemSelected { get; set; }
		public Offset PlaceAnywhere { get; set; }
		public Offset WallPartition { get; set; }
		public BaseOffset ActiveItem { get; set; }
		public Offset ActiveItemPosition { get; set; }
		public Offset MousePosition { get; set; }
	}
}
