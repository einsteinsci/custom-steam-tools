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

		public ITradeCommand Command
		{ get; private set; }

		public PreCommandArgs(string name, ITradeCommand cmd, List<string> args)
		{
			Name = name;
			Command = cmd;
			Args = args;
		}
	}

	public sealed class PostCommandArgs : EventArgs
	{
		public string Name
		{ get; private set; }

		public bool Canceled
		{ get; private set; }

		public List<string> Args
		{ get; private set; }

		public ITradeCommand Command
		{ get; private set; }

		public PostCommandArgs(string name, ITradeCommand cmd, List<string> args)
		{
			Name = name;
			Command = cmd;
			Args = args;
		}
	}

	public sealed class CommandHandler : ReflectiveRegistry<ITradeCommand, TradeCommandAttribute>
	{
		public delegate void PreCommandHandler(object sender, PreCommandArgs e);
		public delegate void PostCommandHandler(object sender, PostCommandArgs e);

		public static CommandHandler Instance
		{ get; private set; }

		public List<ITradeCommand> Commands => registry.Values.ToList();

		public event PreCommandHandler OnPreCommand;
		public event PostCommandHandler OnPostCommand;

		public static void Initialize()
		{
			Instance = new CommandHandler();
		}

		public CommandHandler() : base(true)
		{ }

		public void RunCommand(string commandName, params string[] args)
		{
			List<string> largs = args.ToList();

			ITradeCommand cmd = FindCommand(commandName);
			if (cmd == null)
			{
				VersatileIO.Error("No command found by name '{0}'.", commandName);
				return;
			}

			if (OnPreCommand != null)
			{
				PreCommandArgs e = new PreCommandArgs(commandName, cmd, largs);
				OnPreCommand(this, e);

				if (e.Cancel)
				{
					VersatileIO.Warning("Command canceled.");
					return;
				}
			}

			try
			{
				cmd.RunCommand(this, largs);
			}
			catch (Exception e)
			{
				VersatileIO.Error("There was an error running command '{0}': {1}",
					cmd.RegistryName, e.GetType().Name);
				VersatileIO.Error(e.ToString());
				//throw;
			}

			if (OnPostCommand != null)
			{
				PostCommandArgs e = new PostCommandArgs(commandName, cmd, largs);
				OnPostCommand(this, e);
			}
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
