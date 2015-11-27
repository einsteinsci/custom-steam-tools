using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomSteamTools;
using CustomSteamTools.Lookup;
using CustomSteamTools.Schema;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class PricedViewModel
	{
		public ItemPriceInfo Info
		{ get; private set; }

		public PriceRange Price
		{ get; private set; }

		public string ItemString => Info.Item.ToString(Info.Quality, Info.Australium, Info.Killstreak);
		public string ImageURL => Info.Item.ImageURL;
		public Grid PriceTag => GetPriceTag();

		public PricedViewModel(ItemPriceInfo info, PriceRange range)
		{
			Info = info;
			Price = range;
		}

		public Grid GetPriceTag()
		{
			Grid res = new Grid();
			res.ToolTip = ItemString + ":\n  " + Price.ToString() + "\n  (" + Price.ToStringUSD() + ")";
			res.Margin = new Thickness(3);

			string priceText = Price.ToString();
			bool craftable = Info.Craftable;
			Quality actualQuality = Info.Quality;

			TextBlock textBlock = new TextBlock();
			textBlock.Text = priceText;
			textBlock.FontWeight = FontWeights.SemiBold;
			textBlock.Foreground = new SolidColorBrush(Colors.White);
			textBlock.HorizontalAlignment = HorizontalAlignment.Center;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			textBlock.Margin = new Thickness(6);
			textBlock.FontSize = 16;

			Rectangle rect = new Rectangle();
			rect.StrokeThickness = 2;
			rect.Fill = new SolidColorBrush(actualQuality.ToWPFColor());
			rect.Stroke = new SolidColorBrush(actualQuality.ToWPFBorderColor());
			if (!craftable)
			{
				rect.StrokeDashArray = new DoubleCollection(new double[] { 0, 2, 0 });
			}

			res.Children.Add(rect);
			res.Children.Add(textBlock);

			return res;
		}

		public override string ToString()
		{
			return Info.ToString() + ": " + Price.ToString();
		}
	}
}
