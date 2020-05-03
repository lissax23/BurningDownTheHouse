using System.Windows;

namespace BurningDowntheHouse
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var offset = App.Services.Get<Services.OffsetService>();

			Log.Write(offset.Offsets.ActiveItem);
		}
	}
}
