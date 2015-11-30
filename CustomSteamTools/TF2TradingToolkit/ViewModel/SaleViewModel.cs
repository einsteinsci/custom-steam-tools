using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomSteamTools.Classifieds;
using UltimateUtil;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class SaleViewModel
	{
		public ItemSale Sale
		{ get; private set; }

		public string ImageURL => Sale.Item.ImageURL;

		public Brush QualityBrush => new SolidColorBrush(Sale.Quality.ToWPFBorderColor());

		public Visibility ShowOfferBtn => Sale.CheapestSeller.OfferURL.IsNullOrWhitespace() ? 
			Visibility.Collapsed : Visibility.Visible;

		public string ItemString => Sale.Item.ToString(Sale.Quality, Sale.Pricing.Australium);

		public string PriceString => "@ " + Sale.CheapestSeller.Price.ToString() +
			" (" + Sale.Profit.ToString() + " profit)";

		public string OfferURL => Sale.CheapestSeller.OfferURL;

		public StackPanel Tooltip => GetTooltip();

		public Visibility ShowQuickBuyLabel => Sale.HasQuickDeal ?
			Visibility.Visible : Visibility.Collapsed;

		public SaleViewModel(ItemSale sale)
		{
			Sale = sale;
		}

		public StackPanel GetTooltip()
		{
			StackPanel res = new StackPanel();
			ClassifiedsListing cheapest = Sale.CheapestSeller;

			TextBlock t = new TextBlock();
			t.Text = ItemString;
			t.Foreground = QualityBrush;
			t.FontSize = 16;
			t.FontWeight = FontWeights.SemiBold;
			t.Margin = new Thickness(0, 0, 0, 10);
			res.Children.Add(t);

			if (Sale.HasQuickDeal)
			{
				ClassifiedsListing buyer = Sale.HighestBuyer;
				t = new TextBlock();
				t.Text = "Quick Sale for " + buyer.Price.ToString();
				t.Text += " from " + (buyer.ListerNickname ?? ("#" + buyer.ListerSteamID64));
				t.FontSize = 14;
				t.Foreground = new SolidColorBrush(Colors.Green);
				res.Children.Add(t);
			}

			t = new TextBlock();
			t.Text = "backpack.tf price: " + Sale.Pricing.Pricing.ToString();
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = "Profit: " + Sale.Profit.ToString();
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
