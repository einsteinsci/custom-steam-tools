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
using UltimateUtil;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class PricedViewModel
	{
		public ItemPriceInfo Info
		{ get; private set; }

		public PriceRange Price
		{ get; private set; }

		public string ItemString => GetItemString();
		public string ImageURL => Info.Item.ImageURL;
		public Grid PriceTag => GetPriceTag();

		public string PriceString => Price.ToString();
		public Brush QualityDarkBrush => new SolidColorBrush(Info.Quality.ToWPFBorderColor());

		public string WikiLink => Info.Item.GetWikiLink();
		public string StatsLink => Info.Item.GetStatsLink();

		public StackPanel Tooltip => GetTooltip();

		public PricedViewModel(ItemPriceInfo info, PriceRange range)
		{
			Info = info;
			Price = range;
		}

		public string GetItemString()
		{
			string res = Info.Item.ToString(Info.Quality, Info.Australium, Info.Killstreak);
			if (Info.Unusual != null)
			{
				res += " (" + Info.Unusual.Name + ")";
			}

			return res;
		}

		public StackPanel GetTooltip()
		{
			StackPanel res = new StackPanel();

			TextBlock t = new TextBlock();
			t.Text = Info.Item.ToString(Info.Quality, Info.Australium, Info.Killstreak);
			t.Foreground = new SolidColorBrush(Info.Quality.ToWPFBorderColor());
			t.FontSize = 16;
			t.FontWeight = FontWeights.SemiBold;
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = Info.Item.GetSubtext();
			t.Foreground = new SolidColorBrush(ItemSlotViewModel.SEMI_DARK_GRAY);
			t.FontSize = 12;
			t.Margin = new Thickness(0, 0, 0, 5);
			res.Children.Add(t);

			string desc = Info.Item.Description;
			if (!desc.IsNullOrWhitespace())
			{
				desc = desc.Replace('\t', ' ');
				t = new TextBlock();
				t.Text = desc;
				t.TextWrapping = TextWrapping.Wrap;
				t.MaxWidth = 512;
				t.Margin = new Thickness(0, 0, 0, 5);
				res.Children.Add(t);
			}
			
			if (Info.Unusual != null)
			{
				t = new TextBlock();
				t.Text = "Unusual: " + Info.Unusual.Name + " (" + Info.Unusual.ID + ")";
				t.Foreground = QualityDarkBrush;
				t.Margin = new Thickness(0, 0, 0, 5);
				res.Children.Add(t);
			}

			#region bullets
			t = new TextBlock();
			t.Text = " - Defindex: " + Info.Item.Defindex.ToString();
			res.Children.Add(t);

			if (Info.Item.PlainSlot != ItemSlotPlain.Unused)
			{
				t = new TextBlock();
				t.Text = " - Slot: {0} ({1})".Fmt(Info.Item.PlainSlot, Info.Item.Slot);
				res.Children.Add(t);
			}

			if (Info.Item.ValidClasses.HasItems())
			{
				t = new TextBlock();
				t.Text = " - Classes: " + Info.Item.ValidClasses.ToReadableString(includeBraces: false);
				res.Children.Add(t);
			}

			if (Info.Item.IsCheapWeapon() && Info.Quality == Quality.Unique)
			{
				t = new TextBlock();
				t.Text = " - Drop weapon";
				res.Children.Add(t);
			}

			if (Info.Item.HalloweenOnly || Info.Item.HasHauntedVersion == true)
			{
				t = new TextBlock();
				t.Text = " - Halloween only";
				t.Foreground = new SolidColorBrush(Colors.Teal);
				res.Children.Add(t);
			}
			#endregion

			return res;
		}

		public Grid GetPriceTag()
		{
			Grid res = new Grid();
			res.ToolTip = GetTooltip();
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
