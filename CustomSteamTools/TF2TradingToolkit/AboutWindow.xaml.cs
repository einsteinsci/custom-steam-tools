using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
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

using CustomSteamTools;

using UltimateUtil;

namespace TF2TradingToolkit
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : MahApps.Metro.Controls.MetroWindow
	{
		public string Version
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyFileVersionAttribute ver = asm.GetCustomAttribute<AssemblyFileVersionAttribute>();

				return "Version: " + ver.Version;
			}
		}

		public string LibVersion
		{
			get
			{
				Assembly asm = Assembly.GetAssembly(typeof(Price));
				AssemblyFileVersionAttribute ver = asm.GetCustomAttribute<AssemblyFileVersionAttribute>();

				return "Library Version: " + ver.Version;
			}
		}

		public string Description
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyDescriptionAttribute desc = asm.GetCustomAttribute<AssemblyDescriptionAttribute>();

				return desc.Description;
			}
		}

		public string Copyright
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				AssemblyCopyrightAttribute tm = asm.GetCustomAttribute<AssemblyCopyrightAttribute>();

				return tm.Copyright;
			}
		}

		public string License => Properties.Resources.GPLv3;

		public AboutWindow()
		{
			InitializeComponent();
		}

		private void OkBtn_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Hyperlink hl = sender as Hyperlink;
			if (sender == null)
			{
				return;
			}

			string url = hl.NavigateUri.ToString();

			if (url.IsNullOrWhitespace())
			{
				return;
			}

			Util.OpenLink(url);
		}
	}
}
