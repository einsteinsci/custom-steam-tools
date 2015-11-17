using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Json.ItemDataJson;
using CustomSteamTools.Lookup;
using CustomSteamTools.Utils;

namespace CustomSteamTools.Items
{
	public class Item
	{
		public string UnlocalizedName
		{ get; set; }

		public string Name
		{ get; set; }

		public string ImproperName
		{ get; set; }

		public bool IsProper
		{ get; set; }

		public long ID
		{ get; set; }

		public string Description
		{ get; set; }

		public string Subtext
		{ get { return "Level " + MaxLevel.ToString() + " " + Type; } }

		public string Type
		{ get; set; }

		public Quality DefaultQuality
		{ get; set; }

		public int MinLevel
		{ get; set; }

		public int MaxLevel
		{ get; set; }

		public ItemSlot Slot
		{ get; set; }

		public ItemSlotPlain PlainSlot
		{ get { return Slot.GetPlain(); } }

		public string ItemSetLookup
		{ get; set; }

		public bool HalloweenOnly
		{ get; set; }

		public bool? HasHauntedVersion
		{ get; set; }

		public string ImageURL
		{ get; set; }

		public List<string> Styles
		{ get; private set; }

		public List<PlayerClass> ValidClasses
		{ get; private set; }

		public List<ItemAttribute> Attributes
		{ get; private set; }

		private ItemJson _jsonSrc;

		public Item(ItemJson json, List<ItemAttribute> attsRef)
		{
			UnlocalizedName = json.name;
			Name = (json.proper_name ? "The " : "") + json.item_name;
			ImproperName = json.item_name;
			IsProper = json.proper_name;
			ID = json.defindex;
			Description = json.item_description;
			Type = json.item_type_name;
			DefaultQuality = (Quality)json.item_quality;
			MinLevel = json.min_ilevel;
			MaxLevel = json.max_ilevel;
			Slot = ItemSlots.Parse(json.item_slot);
			ItemSetLookup = json.item_set;
			HalloweenOnly = json.holiday_restriction == ItemJson.HolidayRestrictionTypes.HALLOWEEN;
			ImageURL = json.image_url;
			Styles = json.styles?.ConvertAll((j) => j.name) ?? new List<string>();
			ValidClasses = json.used_by_classes?.ConvertAll((s) => PlayerClasses.Parse(s)) ?? PlayerClasses.All;
			Attributes = json.attributes?.ConvertAll((aas) => attsRef.First((ia) => ia.Name == aas.name)) ?? new List<ItemAttribute>();

			_jsonSrc = json;
		}

		public string GetSubtext()
		{
			if (MinLevel == MaxLevel)
			{
				return "Level " + MinLevel.ToString() + " " + Type;
			}

			return "Level " + MinLevel.ToString() + "-" + MaxLevel.ToString() + " " + Type;
		}

		public bool IsCurrency()
		{
			return ID == Price.REF_DEFINDEX || ID == Price.REC_DEFINDEX || 
				ID == Price.SCRAP_DEFINDEX || ID == Price.KEY_DEFINDEX;
		}

		public Price GetCurrencyPrice()
		{
			if (!IsCurrency())
			{
				throw new ArgumentException("Item must be currency.");
			}

			switch (ID)
			{
				case Price.REF_DEFINDEX:
					return Price.OneRef;
				case Price.REC_DEFINDEX:
					return new Price(0, 0.33);
				case Price.SCRAP_DEFINDEX:
					return new Price(0, 0.11);
				case Price.KEY_DEFINDEX:
					return Price.OneKey;
			}

			return Price.Zero;
		}

		public bool CanBeAustralium()
		{
			if (DataManager.PriceData == null)
			{
				DataManager.TranslatePricingData();
			}

			List<ItemPricing> pricings = DataManager.PriceData.GetAllPriceData(this);
			foreach (ItemPricing p in pricings)
			{
				if (p.Australium)
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			if (obj is Item)
			{
				return (obj as Item).ID == ID;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public bool IsBotkiller()
		{
			return ImproperName.Contains("Botkiller");
		}

		public bool IsSkin()
		{
			return UnlocalizedName.StartsWith("concealedkiller_") ||
				UnlocalizedName.StartsWith("craftsmann_") ||
				UnlocalizedName.StartsWith("teufort_") ||
				UnlocalizedName.StartsWith("powerhouse_");
		}

		public Skin GetSkin()
		{
			return GunMettleSkins.GetSkin(UnlocalizedName);
		}

		public bool IsSupplyCrate()
		{
			if (_jsonSrc.attributes == null)
			{
				return false;
			}

			foreach (AppliedAttributeJson aaj in _jsonSrc.attributes)
			{
				if (aaj.@class == "supply_crate_series")
				{
					return true;
				}
			}

			return false;
		}

		public int? GetSupplyCrateSeries()
		{
			if (!IsSupplyCrate())
			{
				return null;
			}

			foreach (AppliedAttributeJson aaj in _jsonSrc.attributes)
			{
				if (aaj.@class == "supply_crate_series")
				{
					return (int)aaj.value;
				}
			}

			return null; // this line of code will never run
		}

		public bool IsKillstreakKit()
		{
			return _jsonSrc?.tool?.type == "killstreakifier";
		}

		public bool IsStrangifier()
		{
			return _jsonSrc?.tool?.type == "strangifier";
		}

		public long? GetTargetID()
		{
			if (IsKillstreakKit())
			{
				if (_jsonSrc.attributes != null)
				{
					foreach (AppliedAttributeJson aaj in _jsonSrc.attributes)
					{
						if (aaj.@class == "tool_target_item")
						{
							return (long)aaj.value;
						}
					}

					LoggerOld.Log("No target found for killstreakifier '" + UnlocalizedName + "'.", ConsoleColor.Red);
				}
				else
				{
					string targetImpName = UnlocalizedName.Substring(0, UnlocalizedName.IndexOf(" Killstreakifier"));

					foreach (Item i in DataManager.Schema.Items)
					{
						if (i.ImproperName == targetImpName)
						{
							return i.ID;
						}
					}

					LoggerOld.Log("No target found for killstreakifier '" + UnlocalizedName + "'.", ConsoleColor.Red);
				}
			}
			else if (IsStrangifier())
			{
				if (ID == 5633) // strange bacon grease
				{
					return 264; // frying pan
				}

				if (_jsonSrc.attributes != null)
				{
					foreach (AppliedAttributeJson aaj in _jsonSrc.attributes)
					{
						if (aaj.@class == "tool_target_item")
						{
							return (long)aaj.value;
						}
					}

					LoggerOld.Log("No target found for strangifier '" + UnlocalizedName + "'.", ConsoleColor.Red);
				}
				else
				{
					if (UnlocalizedName == "Strangifier")
					{
						return null;
					}

					string targetImpName = UnlocalizedName.CutOffEnd(" Strangifier".Length);

					foreach (Item i in DataManager.Schema.Items)
					{
						if (i.ImproperName == targetImpName)
						{
							return i.ID;
						}
					}
				}
			}

			return null;
		}

		public KillstreakType? GetKillstreakKitTier()
		{
			if (!IsKillstreakKit())
			{
				return null;
			}

			foreach (AppliedAttributeJson att in _jsonSrc.attributes)
			{
				if (att.@class == "killstreak_tier")
				{
					return (KillstreakType)((int)att.value);
				}
			}

			return null;
		}

		public bool IsCheapWeapon()
		{
			if (PlainSlot != ItemSlotPlain.Weapon)
			{
				return false;
			}

			ItemPricing pricing = DataManager.PriceData.GetPriceData(this, Quality.Unique, null, true);
			if (pricing == null)
			{
				return false;
			}

			return (int)(pricing.PriceLow.TotalRefined * 100.0) == 5;
		}
	}
}
