using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Schema;

namespace TF2TradingToolkit.ViewModel
{
	public sealed class UnusualViewModel
	{
		public static readonly UnusualViewModel None = new UnusualViewModel(true);

		public string Name => IsNone ? "None" : Effect.Name;

		public int ID => IsNone ? -1 : Effect.ID;

		public string BpTfURL => IsNone ? null : ("http://backpack.tf/images/440/particles/" + 
			ID.ToString() + "_94x94.png");

		public UnusualEffect Effect
		{ get; private set; }

		public bool IsNone
		{ get; private set; }

		public UnusualViewModel(UnusualEffect fx)
		{
			Effect = fx;
		}

		private UnusualViewModel(bool empty)
		{
			IsNone = empty;
		}
	}
}
