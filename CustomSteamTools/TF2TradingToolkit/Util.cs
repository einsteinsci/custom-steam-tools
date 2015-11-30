using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Schema;
using UltimateUtil;

namespace TF2TradingToolkit
{
	public static class Util
	{
		public static Color ToWPFColor(this Quality q)
		{
			switch (q)
			{
			case Quality.Stock:
				return Colors.LightGray;
			case Quality.Genuine:
				return Colors.Green;
			case Quality.Vintage:
				return Colors.CornflowerBlue;
			case Quality.Unusual:
				return Colors.MediumPurple;
			case Quality.Unique:
				return Colors.Goldenrod;
			case Quality.Community:
			case Quality.SelfMade:
				return Colors.LightGreen;
			case Quality.Valve:
				return Colors.MediumVioletRed;
			case Quality.Strange:
				return new Color() { R = 200, G = 100, B = 0, A = 255 };
			case Quality.Haunted:
				return Colors.Teal;
			case Quality.Collectors:
				return Colors.DarkRed;
			case Quality.Decorated:
				return Colors.CadetBlue;
			default:
				return Colors.DarkGray;
			}
		}
		public static Color ToWPFBorderColor(this Quality q)
		{
			switch (q)
			{
			case Quality.Stock:
				return Colors.DarkGray;
			case Quality.Genuine:
				return Colors.DarkGreen;
			case Quality.Vintage:
				return Colors.MidnightBlue;
			case Quality.Unusual:
				return Colors.DarkViolet;
			case Quality.Unique:
				return Colors.DarkGoldenrod;
			case Quality.Community:
			case Quality.SelfMade:
				return Colors.Green;
			case Quality.Valve:
				return Colors.DarkRed;
			case Quality.Strange:
				return new Color() { R = 110, G = 50, B = 0, A = 255 };
			case Quality.Haunted:
				return Colors.DarkTurquoise;
			case Quality.Collectors:
				return Colors.DarkRed;
			case Quality.Decorated:
				return Colors.SlateBlue;
			default:
				return Colors.Black;
			}
		}

		public static void OpenLink(string url)
		{
			System.Diagnostics.Process.Start(url);
		}

		public static Visibility ToVisibility(this bool b)
		{
			return b ? Visibility.Visible : Visibility.Collapsed;
		}

		public static StackPanel GetTooltip(this DealsFilters filters)
		{
			StackPanel res = new StackPanel();

			TextBlock t = new TextBlock();
			if (filters.Qualities.HasItems())
			{
				t.Text = "Qualities: " + filters.Qualities.ToReadableString(" ", false);
			}
			else
			{
				t.Text = "Any Quality";
			}
			res.Children.Add(t);

			t = new TextBlock();
			if (filters.Slots.HasItems())
			{
				t.Text = "Slots: " + filters.Slots.ToReadableString(" ", false);
			}
			else
			{
				t.Text = "Any Slot";
			}
			res.Children.Add(t);

			t = new TextBlock();
			if (filters.Classes.HasItems())
			{
				t.Text = "Classes: " + filters.Classes.ToReadableString(" ", false);
			}
			else
			{
				t.Text = "Any Class";
			}
			t.Margin = new Thickness(0, 0, 0, 5);
			res.Children.Add(t);

			if (filters.DealsMinProfit != null)
			{
				t = new TextBlock();
				t.Text = "Minimum Profit: " + filters.DealsMinProfit.Value.ToString();
				t.Margin = new Thickness(0, 0, 0, 5);
				res.Children.Add(t);
			}

			if (!filters.AllowAllClass)
			{
				t = new TextBlock();
				t.Text = "No All-Class Items";
				res.Children.Add(t);
			}

			if (filters.Craftable == null)
			{
				t = new TextBlock();
				t.Text = "Any Craftability";
				res.Children.Add(t);
			}
			else if (filters.Craftable == false)
			{
				t = new TextBlock();
				t.Text = "Uncraftable Only";
				res.Children.Add(t);
			}

			if (filters.Halloween == true)
			{
				t = new TextBlock();
				t.Text = "Halloween-Only";
				res.Children.Add(t);
			}
			else if (filters.Halloween == false)
			{
				t = new TextBlock();
				t.Text = "No Halloween Items";
				res.Children.Add(t);
			}

			if (filters.Botkiller == true)
			{
				t = new TextBlock();
				t.Text = "Botkillers Only";
				res.Children.Add(t);
			}
			else if (filters.Botkiller == false)
			{
				t = new TextBlock();
				t.Text = "No Botkillers";
				res.Children.Add(t);
			}

			return res;
		}
	}
}
