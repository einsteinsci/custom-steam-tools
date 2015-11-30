using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CustomSteamTools.Schema;

namespace TF2TradingToolkit
{
	public enum SelectorActionType
	{
		Add,
		Remove
	}

	public delegate void MultiQualitySelectorEventHandler(object sender, MultiQualitySelectorEventArgs e);
	public delegate void MultiSlotSelectorEventHandler(object sender, MultiSlotSelectorEventArgs e);
	public delegate void MultiClassSelectorEventHandler(object sender, MultiClassSelectorEventArgs e);

	public class MultiSelectorEventArgs<TSelect> : EventArgs
	{
		public TSelect Selection
		{ get; private set; }

		public SelectorActionType ActionType
		{ get; private set; }

		public MultiSelectorEventArgs(TSelect selection, SelectorActionType actionType) : base()
		{
			Selection = selection;
			ActionType = actionType;
		}
	}

	public class MultiQualitySelectorEventArgs : MultiSelectorEventArgs<Quality>
	{
		public MultiQualitySelectorEventArgs(Quality quality, SelectorActionType actionType) :
			base(quality, actionType)
		{ }
	}

	public class MultiSlotSelectorEventArgs : MultiSelectorEventArgs<ItemSlotPlain>
	{
		public MultiSlotSelectorEventArgs(ItemSlotPlain slotPlain, SelectorActionType actionType) :
			base(slotPlain, actionType)
		{ }
	}

	public class MultiClassSelectorEventArgs : MultiSelectorEventArgs<PlayerClass>
	{
		public MultiClassSelectorEventArgs(PlayerClass playerClass, SelectorActionType actionType) :
			base(playerClass, actionType)
		{ }
	}
}
