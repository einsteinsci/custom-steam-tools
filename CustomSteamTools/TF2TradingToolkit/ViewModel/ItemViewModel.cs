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

		public string Tooltip => GetItemTooltip(Item);

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

		public static string GetItemTooltip(Item item)
		{
			string nl = Environment.NewLine;

			string res = item.ToString();
			res += nl + "  " + item.GetSubtext();
			res += nl + " - Description: " + item.Description?.Shorten(120).Replace('\n', ' ') ?? "";
			res += nl + " - Defindex: " + item.ID;
			res += nl + " - Slot: {0} ({1})".Fmt(item.PlainSlot, item.Slot);
			res += nl + " - Classes: " + item.ValidClasses.ToReadableString(includeBraces: false);
			if (item.IsSkin())
			{
				Skin skin = item.GetSkin();
				res += nl + " - " + skin.Description;
			}
			if (item.CanBeAustralium())
			{
				res += nl + " - Can be Australium";
			}
			if (item.IsCheapWeapon())
			{
				res += nl + " - Drop weapon";
			}
			if (item.HalloweenOnly || item.HasHauntedVersion == true)
			{
				res += nl + " - Halloween only";
			}

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
			res.ToolTip = Item.ToString(cp.Quality, cp.Pricing?.Australium ?? false) + 
				":\n  " + cp.Price.ToString() + "\n  (" + cp.Price.ToStringUSD() + ")";
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
