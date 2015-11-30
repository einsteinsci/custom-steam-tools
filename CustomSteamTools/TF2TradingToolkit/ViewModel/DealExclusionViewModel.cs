using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomSteamTools.Classifieds;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class DealExclusionViewModel
	{
		public ItemSale Excluded
		{ get; private set; }

		public string Reason
		{ get; private set; }

		public string ReasonReadable
		{
			get
			{
				if (Reason == "PRICEDROPPING")
				{
					return "The price is dropping below the listed bp.tf price.";
				}
				else if (Reason == "NOPROFIT")
				{
					return "There was no profit seen in flipping this item.";
				}
				else if (Reason == "LOWPROFIT")
				{
					return "The profit from flipping this item was too small.";
				}
				else
				{
					return Reason;
				}
			}
		}

		public StackPanel Tooltip => GetTooltip();

		public string ImageURL => Excluded.Item.ImageURL;

		public string ItemString => Excluded.Item.ToString(Excluded.Quality, Excluded.Pricing.Australium);

		public Brush QualityBrush => new SolidColorBrush(Excluded.Quality.ToWPFBorderColor());

		public DealExclusionViewModel(ItemSale excluded, string reason)
		{
			Excluded = excluded;
			Reason = reason;
		}

		public StackPanel GetTooltip()
		{
			StackPanel res = new StackPanel();
			ClassifiedsListing cheapest = Excluded.CheapestSeller;

			TextBlock t = new TextBlock();
			t.Text = ItemString;
			t.Foreground = QualityBrush;
			t.FontSize = 16;
			t.FontWeight = FontWeights.SemiBold;
			t.Margin = new Thickness(0, 0, 0, 10);
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = "Reason for exclusion: " + ReasonReadable;
			t.Foreground = new SolidColorBrush(Colors.Red);
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = "backpack.tf price: " + Excluded.Pricing.Pricing.ToString();
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = "Profit: " + Excluded.Profit.ToString();
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = "Cheapest from " + (cheapest.ListerNickname ?? ("#" + cheapest.ListerSteamID64));
			t.Text += " @ " + cheapest.Price.ToString();
			res.Children.Add(t);

			if (cheapest.Comment != null)
			{
				t = new TextBlock();
				t.Text = cheapest.Comment;
				t.FontStyle = FontStyles.Italic;
				res.Children.Add(t);
			}

			return res;
		}
	}
}
