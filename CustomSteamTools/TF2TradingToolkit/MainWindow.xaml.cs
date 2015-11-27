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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Elysium.Controls.Window
	{
		private InitWindow _initializerWindow;

		public bool AutoExiting
		{ get; private set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		private void RefreshDropDownBtn_Click(object sender, RoutedEventArgs e)
		{
			RefreshDropDownBtn.ContextMenu.IsOpen = true;
		}

		private void RefreshDropDownBtn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			if (AutoExiting)
			{
				return;
			}

			if (RefreshDropDownBtn.ContextMenu == null)
			{
				RefreshDropDownBtn.ContextMenu = FindResource("K_RefreshMenu") as ContextMenu;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_initializerWindow = new InitWindow();
			bool? loaded = _initializerWindow.ShowDialog();

			if (loaded != true)
			{
				AutoExiting = true;
				Close();
			}

			ItemsView.PostLoad();
		}

		private void RefreshAllBtn_Click(object sender, RoutedEventArgs e)
		{
			if (AutoExiting)
			{
				return;
			}

			InputNumberWindow num = new InputNumberWindow("Prompt");
			num.ShowDialog();
		}
	}
}
