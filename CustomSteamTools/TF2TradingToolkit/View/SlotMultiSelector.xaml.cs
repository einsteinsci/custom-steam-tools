using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CustomSteamTools.Schema;

namespace TF2TradingToolkit.View
{
	/// <summary>
	/// Interaction logic for SlotMultiSelector.xaml
	/// </summary>
	public partial class SlotMultiSelector : UserControl
	{
		public HashSet<ItemSlotPlain> SelectedSlots
		{ get; private set; }

		public event MultiSlotSelectorEventHandler SelectionChanged;

		public SlotMultiSelector()
		{
			InitializeComponent();
			SelectedSlots = new HashSet<ItemSlotPlain>();
		}

		private void _fireSelectionChanged(ItemSlotPlain p, SelectorActionType t)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, new MultiSlotSelectorEventArgs(p, t));
			}
		}

		private void _toggle(ItemSlotPlain s)
		{
			if (s == ItemSlotPlain.Action || s == ItemSlotPlain.Unused)
			{
				if (!SelectedSlots.Contains(ItemSlotPlain.Action))
				{
					SelectedSlots.Add(ItemSlotPlain.Action);
					SelectedSlots.Add(ItemSlotPlain.Unused);
					_fireSelectionChanged(ItemSlotPlain.Action, SelectorActionType.Add);
					_fireSelectionChanged(ItemSlotPlain.Unused, SelectorActionType.Add);
				}
				else
				{
					SelectedSlots.Remove(ItemSlotPlain.Action);
					SelectedSlots.Remove(ItemSlotPlain.Unused);
					_fireSelectionChanged(ItemSlotPlain.Action, SelectorActionType.Remove);
					_fireSelectionChanged(ItemSlotPlain.Unused, SelectorActionType.Remove);
				}
			}
			else
			{
				if (!SelectedSlots.Contains(s))
				{
					SelectedSlots.Add(s);
					_fireSelectionChanged(s, SelectorActionType.Add);
				}
				else
				{
					SelectedSlots.Remove(s);
					_fireSelectionChanged(s, SelectorActionType.Remove);
				}
			}
		}

		private void WeaponsBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(ItemSlotPlain.Weapon);
		}

		private void CosmeticsBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(ItemSlotPlain.Cosmetic);
		}

		private void TauntsBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(ItemSlotPlain.Taunt);
		}

		private void OthersBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggle(ItemSlotPlain.Action);
		}
	}
}
