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
	public sealed class ItemViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Item Item
		{ get; private set; }

		public PriceCheckResults PriceCheck
		{ get; private set; }

		public string ImageURL => Item.ImageURL;
		public string Name => Item.ToString();

		public ObservableCollection<CheckedPrice> PriceListings
		{ get; private set; }

		public WrapPanel QualityPriceUI => GetPriceStamps();

		public string Tooltip => GetItemTooltip();

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

			_raisePropertyChanged(nameof(PriceListings));
		}

		public string GetItemTooltip()
		{
			string nl = Environment.NewLine;

			string res = Item.ToString();
			res += nl + "  " + Item.GetSubtext();
			res += nl + "  Description: " + Item.Description?.Shorten(120).Replace('\n', ' ') ?? "";
			res += nl + "  Defindex: " + Item.ID;
			res += nl + "  Slot: {0} ({1})".Fmt(Item.PlainSlot, Item.Slot);
			res += nl + "  Classes: " + Item.ValidClasses.ToReadableString(includeBraces: false);
			if (Item.IsSkin())
			{
				Skin skin = Item.GetSkin();
				res += nl + "  " + skin.Description;
			}
			if (Item.CanBeAustralium())
			{
				res += nl + "  Can be Australium";
			}
			if (Item.IsCheapWeapon())
			{
				res += nl + "  Drop weapon";
			}
			if (Item.HalloweenOnly || Item.HasHauntedVersion == true)
			{
				res += nl + "  Halloween only";
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

		private void _raisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
