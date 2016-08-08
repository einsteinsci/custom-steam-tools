using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomSteamTools.Utils;
using CustomSteamTools.Schema;
using CustomSteamTools.Json.MarketPricesJson;
using CustomSteamTools.Lookup;
using UltimateUtil.UserInteraction;
using CustomSteamTools.Skins;
using CustomSteamTools.Backpacks;

namespace CustomSteamTools.Market
{
	public class MarketPricing
	{
		public bool Failed
		{ get; private set; }

		public string MarketHash
		{ get; private set; }

		public string AsciiHash => Util.Asciify(MarketHash);

		public Item Item
		{ get; private set; }

		public Skin GunMettleSkin
		{ get; private set; }

		public ulong LastUpdated
		{ get; private set; }

		public int Volume
		{ get; private set; }

		public double PriceUSD
		{ get; private set; }

		public Price Price => Price.FromUSD(PriceUSD);

		public KillstreakType Killstreak
		{ get; private set; }

		public Quality Quality
		{ get; private set; }

		public SkinWear? Wear
		{ get; private set; }

		public bool Australium
		{ get; private set; }

		public MarketPricing(string hash, MarketPricingJson json, GameSchema lookup) : 
			this(hash, json.last_updated, json.quantity, json.value / 100.0, lookup)
		{ }

		public MarketPricing(string hash, ulong lastUpdated, int volume, double priceUSD, GameSchema lookup)
		{
			Failed = false;

			MarketHash = hash;
			LastUpdated = lastUpdated;
			Volume = volume;
			PriceUSD = priceUSD;

			if (hash.StartsWith("restriction") ||
				hash.EndsWith("Fabricator") ||				// don't even bother
				hash.Contains("Napolean") ||				// -facepalm-
				hash.StartsWith("Strange Part:") || 
				hash.StartsWith("Strange Filter:") || 
				hash.StartsWith("Strange Cosmetic Part:") ||
				hash == "Naughty Winter Crate Key 2014" ||	// these aren't even usable anymore
				hash == "Nice Winter Crate Key 2014" ||
				hash == "Strange Count Transfer Tool")
			{
				Failed = true;
				return;
			}

			Failed = _processHash(MarketHash, lookup);
		}

		private bool _processHash(string hashStart, GameSchema lookup)
		{
			string hash = hashStart.Trim();

			bool isFestive = hash.StartsWith("Festive ");
			if (isFestive)
			{
				hash = hash.Substring("Festive ".Length);
			}

			#region qualities
			bool hasQualitiesLeft = true;
			Quality? q = null;
			while (hasQualitiesLeft) // Sometimes items glitch into having multiple qualities.
			{						 //   Only the first one counts here.
				Tuple<Quality, string> res = GetQualityFromHash(hash);
				if (q != null && res.Item1 == Quality.Unique)
				{
					hasQualitiesLeft = false;
				}
				else if (q == null)
				{
					q = res.Item1;
				}

				hash = res.Item2;
			}
			Quality = q.Value;
			#endregion qualities

			isFestive = hash.StartsWith("Festive ");
			if (isFestive)
			{
				hash = hash.Substring("Festive ".Length);
			}

			Tuple<KillstreakType, string> ksRes = GetKillstreakFromHash(hash);
			Killstreak = ksRes.Item1;
			string hashPreSkin = ksRes.Item2;

			Tuple<SkinWear?, string> swRes = GetWearFromHash(hashPreSkin);
			Wear = swRes.Item1;
			hash = swRes.Item2;

			#region skin
			foreach (Skin s in WeaponSkins.Skins)
			{
				if (s.GetMarketHash(Wear) == hashPreSkin)
				{
					GunMettleSkin = s;
					Item = s.GetItemForm(lookup);
					return false;
				}
			}

			if (Wear != null)
			{
				VersatileIO.Error("No skin was found for skin '{0}'.", hashPreSkin);
				return true;
			}

			if (isFestive)
			{
				hash = "Festive " + hash;
			}
			#endregion

			if (hash.StartsWith("Australium") && hashStart != "Australium Gold")
			{
				Australium = true;
				hash = hash.Substring("Australium ".Length); // the space is intentional
			}

			// items with "Vintage" in the actual name
			else if (hash == "Merryweather" || hash == "Tyrolean")
			{
				hash = "Vintage " + hash;
			}
			// item with "Haunted" in the actual name
			else if (hash == "Hat")
			{
				hash = "Haunted " + hash;
			}

			// make up your mind.
			if (hash == "Eternaween")
			{
				hash = "Enchantment: Eternaween";
			}
			else if (hash == "RoboCrate Key") // general inconsistencies with robocrate keys
			{
				hash = "Robo Community Crate Key";
			}
			else if (hash == "A Random RoboKey Gift")
			{
				hash = "A Random Robo Community Crate Key Gift";
			}
			else if (hash == "Pile of RoboKey Gifts")
			{
				hash = "Pile of Robo Community Crate Key Gifts";
			}

			if (hash.Contains("Creature’s Grin")) // yes, that is a stylized apostrophe
			{
				Item = lookup.GetItem(30525); // do it manually.
			}

			if (Item == null)
			{
				foreach (Item i in lookup.Items)
				{
					if (i.ImproperName.ToLower() == hash.ToLower())
					{
						Item = i;
						break;
					}
				}
			}

			if (Item == null) // again
			{

				// nice goin valve...
				if (hash.StartsWith("The "))
				{
					hash = hash.Substring("The ".Length);

					foreach (Item i in lookup.Items)
					{
						if (i.ImproperName.ToLower() == hash.ToLower())
						{
							Item = i;
							return false;
						}
					}

					VersatileIO.Error("No item was found matching '{0}'.", hashStart);
					return true;
				}
				if (hash.EndsWith("Strangifier") || hashStart == "Strange Bacon Grease")
				{
					// screw these
					return true;

					#region strangifier
#pragma warning disable CS0162
					string targetName = hashStart.CutOffEnd(" Strangifier".Length);
					foreach (Item i in lookup.Items)
					{
						if (!i.IsStrangifier())
						{
							continue;
						}

						long? targetID = i.GetTargetID();
						if (targetID == null)
						{
							return true;
						}

						if (lookup.GetItem(targetID.Value).ImproperName == targetName)
						{
							Item = i;
							break;
						}
					}
#pragma warning restore CS0162
					#endregion
				}
				else if (hash.EndsWith("Kit") && Killstreak.HasKillstreak())
				{
					// and these
					return true;

					#region killstreak kit
#pragma warning disable CS0162
					string targetName = hashStart.CutOffEnd(" Kit".Length);
					foreach (Item i in lookup.Items)
					{
						if (!i.IsKillstreakKit())
						{
							continue;
						}

						// What is with these ones acting so funky?
						if (i.UnlocalizedName.EndsWith("Killstreakifier Basic"))
						{
							continue;
						}

						long? targetID = i.GetTargetID();
						if (targetID == null)
						{
							return true;
						}

						if (lookup.GetItem(targetID.Value).ImproperName == targetName)
						{
							Item = i;
							break;
						}
					}
#pragma warning restore CS0162
					#endregion killstreak kit
				}
				else if (hash.Contains("Series #")) // it's a box!
				{
					int numSign = hash.LastIndexOf('#');
					string sNum = hash.Substring(numSign + 1);
					int n = int.Parse(sNum);

					foreach (Item i in lookup.Items)
					{
						if (!i.IsSupplyCrate())
						{
							continue;
						}

						if (i.GetSupplyCrateSeries().Value == n)
						{
							Item = i;
							break;
						}
					}
				}
				else if (hash.EndsWith("Chemistry Set"))
				{
					// Skip Collector's Chemistry Sets
					return true;
				}
				else
				{
					VersatileIO.Error("No item was found matching '{0}'.", hashStart);
					return true;
				}
			}

			return false;
		}

		public static Tuple<Quality, string> GetQualityFromHash(string hashIn)
		{
			Quality resq = Quality.Valve;
			string hash = hashIn;

			const string DECORATED_GLITCH = "Decorated Weapon";
			if (hash.StartsWith(DECORATED_GLITCH))
			{
				resq = Quality.Decorated;
				hash = hash.Substring(DECORATED_GLITCH.Length);
			}
			else if (hash.StartsWith(Quality.Strange.ToReadableString()))
			{
				resq = Quality.Strange;
				hash = hash.Substring(Quality.Strange.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.Genuine.ToReadableString()))
			{
				resq = Quality.Genuine;
				hash = hash.Substring(Quality.Genuine.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.Vintage.ToReadableString()))
			{
				resq = Quality.Vintage;
				hash = hash.Substring(Quality.Vintage.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.Collectors.ToReadableString()))
			{
				resq = Quality.Collectors;
				hash = hash.Substring(Quality.Collectors.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.Haunted.ToReadableString()))
			{
				resq = Quality.Haunted;
				hash = hash.Substring(Quality.Haunted.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.SelfMade.ToReadableString())) // it happens
			{
				resq = Quality.SelfMade;
				hash = hash.Substring(Quality.SelfMade.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(Quality.Unusual.ToReadableString()))
			{
				resq = Quality.Unusual;
				hash = hash.Substring(Quality.Unusual.ToReadableString().Length + 1);
			}
			else
			{
				resq = Quality.Unique;
			}

			return new Tuple<Quality, string>(resq, hash);
		}

		public static Tuple<KillstreakType, string> GetKillstreakFromHash(string hashIn)
		{
			KillstreakType ks = KillstreakType.None;
			string hash = hashIn;

			if (hash.StartsWith(KillstreakType.Professional.ToReadableString()))
			{
				ks = KillstreakType.Professional;
				hash = hash.Substring(KillstreakType.Professional.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(KillstreakType.Specialized.ToReadableString()))
			{
				ks = KillstreakType.Specialized;
				hash = hash.Substring(KillstreakType.Specialized.ToReadableString().Length + 1);
			}
			else if (hash.StartsWith(KillstreakType.Basic.ToReadableString()))
			{
				ks = KillstreakType.Basic;
				hash = hash.Substring(KillstreakType.Basic.ToReadableString().Length + 1);
			}

			return new Tuple<KillstreakType, string>(ks, hash);
		}

		public static Tuple<SkinWear?, string> GetWearFromHash(string hashIn)
		{
			SkinWear? wear = null;
			string hash = hashIn;

			for (SkinWear w = SkinWear.FactoryNew; w <= SkinWear.BattleScarred; w++)
			{
				if (hash.EndsWith(w.WithParentheses()))
				{
					wear = w;
					hash = hash.CutOffEnd(w.WithParentheses().Length + 1);
					break;
				}
			}

			return new Tuple<SkinWear?, string>(wear, hash);
		}

		public bool MatchesInstance(ItemInstance inst)
		{
			if (inst.Item != Item)
			{
				return false;
			}

			if (inst.GetKillstreak() != Killstreak)
			{
				return false;
			}

			if (inst.Quality != Quality)
			{
				return false;
			}

			return true;
		}

		public static string GetMarketHash(Item item, KillstreakType ks, Quality quality)
		{
			string s_q = quality.ToReadableString();
			if (s_q != "")
			{
				s_q += " ";
			}

			string s_ks = ks.ToReadableString();
			if (s_ks != "")
			{
				s_ks += " ";
			}

			return s_q + s_ks + item.ImproperName;
		}

		public override string ToString()
		{
			return MarketHash;
		}
	}
}
