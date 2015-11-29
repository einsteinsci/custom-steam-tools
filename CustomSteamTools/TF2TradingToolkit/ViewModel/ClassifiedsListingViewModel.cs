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
	public sealed class ClassifiedsListingViewModel
	{
		public ClassifiedsListing Listing
		{ get; private set; }

		public string ListingString => Listing.Price.ToString() + " from " +
			Listing.ListerNickname ?? Listing.ListerSteamID64;

		public string OfferLink => Listing.OfferURL;

		public string Comment => Listing.Comment ?? "(No comment)";

		public StackPanel Tooltip => GetTooltip();

		public string ImageURL => Listing.ItemInstance.Item.ImageURL;

		public Visibility ShowOfferBtn => Listing.OfferURL != null ? 
			Visibility.Visible : Visibility.Collapsed;

		public ClassifiedsListingViewModel(ClassifiedsListing listing)
		{
			Listing = listing;
		}

		public StackPanel GetTooltip()
		{
			StackPanel res = new StackPanel();

			TextBlock t = new TextBlock();
			t.Text = Listing.ItemInstance.ToString(false);
			t.FontSize = 16;
			t.Margin = new Thickness(0, 0, 0, 10);
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = Listing.ItemInstance.GetSubtext();
			t.Margin = new Thickness(0, 0, 0, 5);
			res.Children.Add(t);

			if (Listing.Comment != null)
			{
				t = new TextBlock();
				t.Text = Comment;
				t.FontStyle = FontStyles.Italic;
				t.Foreground = new SolidColorBrush(ItemSlotViewModel.SEMI_DARK_GRAY);
				res.Children.Add(t);
			}

			return res;
		}

		public override string ToString()
		{
			return Listing.ToString();
		}
	}
}
