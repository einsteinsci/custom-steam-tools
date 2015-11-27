using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CustomSteamTools.Schema;

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
				return Colors.Navy;
			case Quality.Unusual:
				return Colors.Purple;
			case Quality.Unique:
				return Colors.Goldenrod;
			case Quality.Community:
			case Quality.SelfMade:
				return Colors.LightGreen;
			case Quality.Valve:
				return Colors.MediumVioletRed;
			case Quality.Strange:
				return new Color() { R = 220, G = 100, B = 0, A = 255 };
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
				return Colors.Navy;
			default:
				return Colors.Black;
			}
		}
	}
}
