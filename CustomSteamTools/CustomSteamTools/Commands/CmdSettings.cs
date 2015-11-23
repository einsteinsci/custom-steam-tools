using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdSettings : ITradeCommand
	{
		public string[] Aliases => new string[] { "settings", "options" };

		public string Description => "Change one of the application settings.";

		public string RegistryName => "settings";

		public string Syntax => "settings {settingName} {value}";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			throw new NotImplementedException();
		}

		public void ChangeSetting(string setting, object value)
		{

		}
	}
}
