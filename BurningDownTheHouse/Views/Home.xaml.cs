using BurningDownTheHouse.Services;
using ConceptMatrix;
using ConceptMatrix.Injection.Offsets;
using PropertyChanged;
using System.Windows.Controls;

namespace BurningDownTheHouse.Views
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	[AddINotifyPropertyChangedInterface]
	public partial class Home : UserControl
	{
		public bool PlaceAnywhere { get; set; }

		[AlsoNotifyFor(nameof(PlaceAnywhere))]
		[DependsOn(nameof(PlaceAnywhere))]
		public bool WallPartition { get => PlaceAnywhere; set => PlaceAnywhere = value; }
		public Vector Position { get; set; }

		private IMemory<Vector> itemPosition;
		private readonly IMemory<bool> placeAnywhere;
		private readonly IMemory<bool> wallPartition;

		public Home()
		{
			this.InitializeComponent();
			this.DataContext = this;

			// Get the selection service.
			var selection = App.Services.Get<SelectedItemService>();
			// Listen for when the selected item changes.
			selection.SelectionChanged += this.SelectionChanged;

			// Get the offsets service.
			var offsets = App.Services.Get<OffsetsService>();

			placeAnywhere = new BaseOffset<bool>(offsets.Get("PlaceAnywhere")).GetMemory();
			wallPartition = new BaseOffset<bool>(offsets.Get("WallPartition")).GetMemory();

			placeAnywhere.Bind(this, nameof(PlaceAnywhere));
			wallPartition.Bind(this, nameof(WallPartition));
		}

		private void SelectionChanged(IMemory<Vector> itemPosition)
		{
			// Unbind the existing memory (not sure if needed).
			this.itemPosition.UnBind(this, nameof(Position));
			// Set the new item position and bind it.
			this.itemPosition = itemPosition;
			this.itemPosition.Bind(this, nameof(Position));
		}
	}
}
