using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateUtil.Registries;

namespace CustomSteamTools.Commands
{
	public interface ITradeCommand : IRegisterable
	{
		string[] Aliases
		{ get; }

		string Description
		{ get; }

		string Syntax
		{ get; }

		void RunCommand(CommandHandler sender, List<string> args);
	}
}
