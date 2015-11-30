using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CustomSteamTools;
using CustomSteamTools.Backpacks;
using CustomSteamTools.Schema;
using UltimateUtil;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class ItemSlotViewModel
	{
		public ItemInstance Item
		{ get; private set; }

		public string ImageURL => Item?.Item.ImageURL;
		public string Title => Item?.ToString(true);
		public Thickness ImageMargin => Item != null ? new Thickness(10, 20, 10, 10) : new Thickness(0);

		public static readonly Color DARK_DARK_GRAY = new Color() { R = 20, G = 20, B = 20, A = 255 };
		public static readonly Color SEMI_DARK_GRAY = new Color() { R = 80, G = 80, B = 80, A = 255 };

		public SolidColorBrush BorderBrush => new SolidColorBrush(Item?.Quality.ToWPFBorderColor() ?? DARK_DARK_GRAY);
		public SolidColorBrush TextColor => new SolidColorBrush(Item?.Quality.ToWPFColor() ?? Colors.White);

		public bool Tradable => Item?.Tradable ?? false;
		public SolidColorBrush TradableBrush => new SolidColorBrush(Tradable ? Colors.White : Colors.Salmon);

		public DoubleCollection BorderDash
		{
			get
			{
				if (Item == null || Item.Craftable || !Item.Tradable)
				{
					return new DoubleCollection();
				}
				else
				{
					return new DoubleCollection(new double[] { 0, 2, 0 });
				}
			}
		}

		public string PriceString
		{ get; private set; }

		public StackPanel Tooltip
		{
			get
			{
				if (Item == null)
				{
					return null;
				}

				return GetTooltip();
			}
		}

		public ItemSlotViewModel(ItemInstance inst)
		{
			Item = inst;

			if (Item == null)
			{
				PriceString = "";
			}
			else if (Item.Tradable)
			{
				PriceRange? p = PriceChecker.GetPrice(Item);
				if (p != null)
				{
					PriceString = p.ToString();
				}
				else
				{
					PriceString = "Price Unknown";
				}
			}
			else
			{
				PriceString = "Not Tradable";
			}
		}

		public StackPanel GetTooltip()
		{
			StackPanel res = new StackPanel();

			TextBlock t = new TextBlock();
			t.Text = Item.ToString();
			t.Foreground = new SolidColorBrush(Item.Quality.ToWPFBorderColor());
			t.FontSize = 16;
			t.FontWeight = FontWeights.SemiBold;
			res.Children.Add(t);

			t = new TextBlock();
			t.Text = Item.GetSubtext();
			t.Foreground = new SolidColorBrush(SEMI_DARK_GRAY);
			t.FontSize = 12;
			t.Margin = new Thickness(0, 0, 0, 5);
			res.Children.Add(t);

			string desc = Item.GetDescription();
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
			t.Text = " - Defindex: " + Item.Item.ID.ToString();
			res.Children.Add(t);

			if (Item.Item.PlainSlot != ItemSlotPlain.Unused)
			{
				t = new TextBlock();
				t.Text = " - Slot: {0} ({1})".Fmt(Item.Item.PlainSlot, Item.Item.Slot);
				res.Children.Add(t);
			}

			if (Item.Item.ValidClasses.HasItems())
			{
				t = new TextBlock();
				t.Text = " - Classes: " + Item.Item.ValidClasses.ToReadableString(includeBraces: false);
				res.Children.Add(t);
			}

			if (Item.Item.CanBeAustralium())
			{
				t = new TextBlock();
				t.Text = " - Can be Australium";
				t.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
				res.Children.Add(t);
			}

			if (Item.Item.IsCheapWeapon() && Item.Quality == Quality.Unique)
			{
				t = new TextBlock();
				t.Text = " - Drop weapon";
				res.Children.Add(t);
			}

			if (Item.Item.HalloweenOnly || Item.Item.HasHauntedVersion == true)
			{
				t = new TextBlock();
				t.Text = " - Halloween only";
				t.Foreground = new SolidColorBrush(Colors.Teal);
				res.Children.Add(t);
			}
			#endregion

			return res;
		}
	}
}
