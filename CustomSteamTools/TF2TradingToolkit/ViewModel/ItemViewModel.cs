using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomSteamTools;
using CustomSteamTools.Commands;
using CustomSteamTools.Lookup;
using CustomSteamTools.Market;
using CustomSteamTools.Schema;
using CustomSteamTools.Skins;
using TF2TradingToolkit.View;
using UltimateUtil;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class ItemViewModel
	{
		public Item Item
		{ get; private set; }

		public PriceCheckResults PriceCheck
		{ get; private set; }

		public string ImageURL => Item.ImageURL;
		public string Name => Item.ToString();

		public ObservableCollection<CheckedPrice> PriceListings
		{ get; private set; }

		public WrapPanel QualityPriceUI => GetPriceStamps();

		public StackPanel Tooltip => GetItemTooltip();

		public string WikiLink => Item.GetWikiLink();
		public string StatsLink => Item.GetStatsLink();

		public ItemViewModel(Item item, QualitySelector selector)
		{
			Item = item;

			PriceCheck = CmdPriceCheck.GetPriceCheckResults(item);
			PriceListings = new ObservableCollection<CheckedPrice>();
			bool hasUniqueCraftable = false, hasUniqueNoncraftable = false;
			foreach (CheckedPrice p in PriceCheck.NonUnusuals)
			{
				if (hasUniqueCraftable && p.Quality == Quality.Unique && p.Pricing.Craftable)
				{
					continue;
				}

				if (hasUniqueNoncraftable && p.Quality == Quality.Unique && !p.Pricing.Craftable)
				{
					continue;
				}

				if (!p.Tradable)
				{
					continue;
				}

				PriceListings.Add(p);
			}
		}

		public StackPanel GetItemTooltip()
		{
			StackPanel res = new StackPanel();

			TextBlock t = new TextBlock();
			t.Text = Item.ToString();
			t.Foreground = new SolidColorBrush(Item.DefaultQuality.ToWPFBorderColor());
			t.FontSize = 16;
			t.FontWeight = FontWeights.SemiBold;
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = Item.GetSubtext();
			t.Foreground = new SolidColorBrush(ItemSlotViewModel.SEMI_DARK_GRAY);
			t.FontSize = 12;
			t.Margin = new Thickness(0, 0, 0, 5);
			res.Children.Add(t);

			string desc = Item.Description;
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

			#region bullets
			t = new TextBlock();
			t.Text = " - Defindex: " + Item.Defindex.ToString();
			res.Children.Add(t);

			if (Item.PlainSlot != ItemSlotPlain.Unused)
			{
				t = new TextBlock();
				t.Text = " - Slot: {0} ({1})".Fmt(Item.PlainSlot, Item.Slot);
				res.Children.Add(t);
			}

			if (Item.ValidClasses.HasItems())
			{
				t = new TextBlock();
				t.Text = " - Classes: " + Item.ValidClasses.ToReadableString(includeBraces: false);
				res.Children.Add(t);
			}

			if (Item.CanBeAustralium())
			{
				t = new TextBlock();
				t.Text = " - Can be Australium";
				t.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
				res.Children.Add(t);
			}

			if (Item.IsCheapWeapon())
			{
				t = new TextBlock();
				t.Text = " - Drop weapon";
				res.Children.Add(t);
			}

			if (Item.HalloweenOnly || Item.HasHauntedVersion == true)
			{
				t = new TextBlock();
				t.Text = " - Halloween only";
				t.Foreground = new SolidColorBrush(Colors.Teal);
				res.Children.Add(t);
			}
			#endregion

			return res;
		}

		public WrapPanel GetPriceStamps()
		{
			WrapPanel res = new WrapPanel();
			res.Orientation = Orientation.Horizontal;

			CheckedPrice uniqueCraftable = PriceCheck.All.FirstOrDefault(
				(_cp) => _cp.Craftable && _cp.Quality == Quality.Unique);
			CheckedPrice uniqueUncraftable = PriceCheck.All.FirstOrDefault(
				(_cp) => !_cp.Craftable && _cp.Quality == Quality.Unique);

			if (uniqueCraftable != null)
			{
				Grid stamp = GetPriceStamp(uniqueCraftable);
				res.Children.Add(stamp);
			}
			if (uniqueUncraftable != null)
			{
				Grid stamp = GetPriceStamp(uniqueUncraftable);
				res.Children.Add(stamp);
			}

			foreach (CheckedPrice cp in PriceCheck.Others)
			{
				Grid stamp = GetPriceStamp(cp);
				res.Children.Add(stamp);
			}

			return res;
		}

		public Grid GetPriceStamp(CheckedPrice cp)
		{
			Grid res = new Grid();
			ItemPriceInfo info = new ItemPriceInfo(cp.Pricing.Item, cp.Quality);
			PricedViewModel pvm = new PricedViewModel(info, cp.Price);
			res.ToolTip = pvm.Tooltip;
			res.Margin = new Thickness(2);

			string priceText = cp.Price.ToString();
			bool craftable = cp.Craftable;
			Quality actualQuality = cp.Quality;

			TextBlock textBlock = new TextBlock();
			textBlock.Text = priceText;
			textBlock.FontWeight = FontWeights.SemiBold;
			textBlock.Foreground = new SolidColorBrush(Colors.White);
			textBlock.HorizontalAlignment = HorizontalAlignment.Center;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			textBlock.Margin = new Thickness(3);
			textBlock.FontSize = 10;

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
	}
}
