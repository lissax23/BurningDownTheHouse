using BurningDownTheHouse.Services;
using ConceptMatrix;
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
		public Vector Position { get; set; }

		private IMemory<Vector> itemPosition;

		public Home()
		{
			this.InitializeComponent();
			this.DataContext = this;

			// Get the selection service.
			var selection = App.Services.Get<SelectedItemService>();
			// Listen for when the selected item changes.
			selection.SelectionChanged += this.SelectionChanged;
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
