using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Utils;
using UltimateUtil;
using UltimateUtil.Fluid;
using UltimateUtil.Registries;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TradeCommandAttribute : Attribute
	{ }

	public sealed class PreCommandArgs : EventArgs
	{
		public string Name
		{ get; private set; }

		public bool Cancel
		{ get; set; }

		public List<string> Args
		{ get; private set; }

		public PreCommandArgs(string name, List<string> args)
		{
			Name = name;
			Args = args;
		}
	}

	public sealed class CommandHandler : ReflectiveRegistry<ITradeCommand, TradeCommandAttribute>
	{
		public delegate bool PreCommandHandler(object sender, PreCommandArgs e);

		public static CommandHandler Instance
		{ get; private set; }

		public List<ITradeCommand> Commands => registry.Values.ToList();

		public event PreCommandHandler OnPreCommand;

		public static void Initialize()
		{
			Instance = new CommandHandler();
		}

		public CommandHandler() : base(true)
		{ }

		public void RunCommand(string command, params string[] args)
		{
			List<string> largs = args.ToList();
			if (OnPreCommand != null)
			{
				PreCommandArgs e = new PreCommandArgs(command, largs);
				OnPreCommand(this, e);

				if (e.Cancel)
				{
					LoggerOld.Log("Command canceled.", ConsoleColor.Yellow);
					return;
				}
			}

			ITradeCommand cmd = FindCommand(command);
			if (cmd == null)
			{
				LoggerOld.Log("No command found by name '{0}'.".Fmt(command), ConsoleColor.Red);
				return;
			}

			cmd.RunCommand(this, largs);
		}

		public ITradeCommand FindCommand(string command)
		{
			ITradeCommand cmd = null;
			foreach (KeyValuePair<string, ITradeCommand> kvp in registry)
			{
				if (kvp.Key.EqualsIgnoreCase(command))
				{
					cmd = kvp.Value;
					break;
				}

				foreach (string a in kvp.Value.Aliases)
				{
					if (a.EqualsIgnoreCase(command))
					{
						cmd = kvp.Value;
						break;
					}
				}
			}

			return cmd;
		}
	}
}
