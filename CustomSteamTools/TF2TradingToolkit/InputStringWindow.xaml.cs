using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for InputStringWindow.xaml
	/// </summary>
	public partial class InputStringWindow : Elysium.Controls.Window
	{
		public string InputResult => InputBox?.Text ?? "";

		public InputStringWindow(string prompt)
		{
			Title = prompt;
			InitializeComponent();
		}

		private void OKBtn_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
