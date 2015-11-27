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
		public string Name => Effect.Name;

		public int ID => Effect.ID;

		public string BpTfURL => "http://backpack.tf/images/440/particles/" + ID.ToString() + "_94x94.png";

		public UnusualEffect Effect
		{ get; private set; }

		public UnusualViewModel(UnusualEffect fx)
		{
			Effect = fx;
		}
	}
}
