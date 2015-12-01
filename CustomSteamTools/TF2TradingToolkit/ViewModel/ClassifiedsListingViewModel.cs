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

namespace TF2TradingToolkit.ViewModel
{
	public sealed class ClassifiedsListingViewModel
	{
		public static ClassifiedsListingViewModel DownloadFailed => _downloadFailed;
		private static ClassifiedsListingViewModel _downloadFailed = new ClassifiedsListingViewModel(true);

		public ClassifiedsListing Listing
		{ get; private set; }

		public Brush QualityColorBrush
		{
			get
			{
				if (Failed)
				{
					return new SolidColorBrush(Colors.Red);
				}

				return new SolidColorBrush(Listing.ItemInstance.Quality.ToWPFBorderColor());
			}
		}

		public string ListingString
		{
			get
			{
				if (Failed)
				{
					return "Download Failed";
				}

				return Listing.Price.ToString() + " from " +
					Listing.ListerNickname ?? Listing.ListerSteamID64;
			}
		}

		public Visibility ShowContextMenu => (!Failed).ToVisibility();

		public string OfferLink => Listing?.OfferURL;

		public string WikiLink => Listing?.ItemInstance.Item.GetWikiLink();
		public string StatsLink => Listing?.ItemInstance.Item.GetStatsLink();

		public string Comment
		{
			get
			{
				if (Failed)
				{
					return "";
				}

				return Listing.Comment ?? "(No comment)";
			}
		}

		public StackPanel Tooltip => GetTooltip();

		public string ImageURL => Listing?.ItemInstance.Item.ImageURL;

		public Visibility ShowOfferBtn
		{
			get
			{
				if (Failed)
				{
					return Visibility.Collapsed;
				}

				return Listing.OfferURL != null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool Failed
		{ get; private set; }

		public ClassifiedsListingViewModel(ClassifiedsListing listing)
		{
			Listing = listing;
		}

		private ClassifiedsListingViewModel(bool failed)
		{
			Failed = failed;
		}

		public StackPanel GetTooltip()
		{
			if (Failed)
			{
				return null;
			}

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

			UnusualEffect fx = Listing.ItemInstance.GetUnusual();
			if (fx != null)
			{
				t = new TextBlock();
				t.Text = "Unusual: " + fx.Name + " (" + fx.ID + ")";
				t.Foreground = QualityColorBrush;
				res.Children.Add(t);
			}

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
