using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Utils;
using Newtonsoft.Json;
using UltimateUtil;
using UltimateUtil.UserInteraction;

namespace CustomSteamTools.Commands
{
	[TradeCommand]
	public class CmdSettings : ITradeCommand
	{
		public string[] Aliases => new string[] { "settings", "options" };

		public string Description => "Change or list the application settings.";

		public string RegistryName => "settings";

		public string Syntax => "settings {list | settingName} [value]";

		public void RunCommand(CommandHandler sender, List<string> args)
		{
			if (args.IsEmpty())
			{
				VersatileIO.Error("Syntax: " + Syntax);
				return;
			}

			Type settingsType = typeof(Settings);
			List<PropertyInfo> props = new List<PropertyInfo>();
			foreach (PropertyInfo p in settingsType.GetProperties())
			{
				if (!p.HasAttribute<JsonIgnoreAttribute>())
				{
					props.Add(p);
				}
			}

			if (args[0].EqualsIgnoreCase("list"))
			{
				VersatileIO.Success("Available Settings:");
				foreach (PropertyInfo p in props)
				{
					object val = p.GetValue(Settings.Instance);
					VersatileIO.Info("  {0} ({1}): {2}", p.Name, p.PropertyType.Name, val.ToString());
					
					SettingAttribute satt = p.GetCustomAttribute<SettingAttribute>();
					if (satt != null)
					{
						VersatileIO.WriteLine("  - " + satt.Meaning, ConsoleColor.Gray);
					}
				}
			}
			else
			{
				PropertyInfo affectedProp = null;
				foreach (PropertyInfo p in props)
				{
					if (p.Name.EqualsIgnoreCase(args[0]))
					{
						affectedProp = p;
					}
				}

				if (affectedProp == null)
				{
					VersatileIO.Error("No setting found by the name {0}.", args[0]);
					return;
				}

				if (args.Count == 1)
				{
					object val = affectedProp.GetValue(Settings.Instance);
					VersatileIO.Info("  {0} ({1}): {2}", affectedProp.Name,
						affectedProp.PropertyType.Name, val.ToString());

					SettingAttribute satt = affectedProp.GetCustomAttribute<SettingAttribute>();
					if (satt != null)
					{
						VersatileIO.WriteLine("  - " + satt.Meaning, ConsoleColor.Gray);
					}
				}
				else
				{
					Type pType = affectedProp.PropertyType;

					try
					{
						object val = Convert.ChangeType(args[1], pType);
						affectedProp.SetValue(Settings.Instance, val);
					}
					catch (FormatException)
					{
						VersatileIO.Error("Invalid {0}: {1}.", pType.Name, args[1]);
						return;
					}
					catch (InvalidCastException)
					{
						VersatileIO.Error("Cannot convert from 'string' to '{0}'.", pType.Name);
						return;
					}
				}
			}
		}
	}
}
