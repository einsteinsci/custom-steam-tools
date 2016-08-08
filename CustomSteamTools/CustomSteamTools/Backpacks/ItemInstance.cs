using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Classifieds;
using CustomSteamTools.Json.BackpackDataJson;
using CustomSteamTools.Lookup;
using CustomSteamTools.Market;
using CustomSteamTools.Schema;
using CustomSteamTools.Skins;

namespace CustomSteamTools.Backpacks
{
	public class ItemInstance
	{
		public ushort BackpackSlot
		{ get; set; }

		public bool IsNewToBackpack
		{ get; private set; }

		public ulong InstanceID
		{ get; private set; }

		public ulong OriginalInstanceID
		{ get; private set; }

		public Item Item
		{ get; private set; }

		public int Level
		{ get; private set; }

		public int Count
		{ get; private set; }

		public bool Tradable
		{ get; private set; }

		public bool Craftable
		{ get; private set; }

		public Quality Quality
		{ get; private set; }

		public string CustomName
		{ get; private set; }

		public string CustomDescription
		{ get; private set; }

		public List<AppliedInstanceAttribute> Attributes
		{ get; private set; }

		public ItemInstance WrappedItem
		{ get; private set; }

		public int Style
		{ get; private set; }

		public ItemInstance(ItemInstanceJson json, GameSchema reference)
		{
			InstanceID = json.id;
			OriginalInstanceID = json.original_id;
			Item = reference.GetItem(json.defindex);
			Level = json.level;
			Count = json.quantity;
			Tradable = !json.flag_cannot_trade;
			Craftable = !json.flag_cannot_craft;
			Quality = (Quality)json.quality;
			CustomName = json.custom_name;
			CustomDescription = json.custom_desc;
			Style = json.style;

			InventoryLocationFlags flags = (InventoryLocationFlags)json.inventory;
			BackpackSlot = flags.GetBackpackPos();
			IsNewToBackpack = flags.IsNewItem();

			Attributes = new List<AppliedInstanceAttribute>();
			if (json.attributes != null)
			{
				foreach (AppliedItemInstanceAttributeJson aiiaj in json.attributes)
				{
					Attributes.Add(new AppliedInstanceAttribute(aiiaj));
				}
			}

			if (json.contained_item != null)
			{
				WrappedItem = new ItemInstance(json.contained_item, reference);
			}
		}

		public ItemInstance(Item item, ulong instanceID, int level, Quality quality = Quality.Unique, bool craftable = true,
			string customName = null, string customDesc = null, ulong? originalInstance = null, bool tradable = true)
		{
			Item = item;
			InstanceID = instanceID;
			Level = level;
			Quality = quality;
			Craftable = craftable;
			Tradable = tradable;
			CustomName = customName;
			CustomDescription = customDesc;
			OriginalInstanceID = originalInstance.HasValue ? originalInstance.Value : instanceID;
			IsNewToBackpack = true;
			BackpackSlot = 0;

			Attributes = new List<AppliedInstanceAttribute>();
		}

		public string TitleQuick(bool includeKillstreak = false)
		{
			if (CustomName != null)
			{
				return "'" + CustomName + "'";
			}

			string res = Item.ToString();

			if (GetKillstreak() != KillstreakType.None)
			{
				res = GetKillstreak().ToReadableString() + " " + res;
			}

			string q = Quality.ToReadableString();
			if (q != "")
			{
				res = q + " " + res;
			}

			return res;
		}

		public string GetSubtext()
		{
			if (Item.IsSkin())
			{
				return Item.GetSkin().Description;
			}

			return "Level " + Level + " " + Item.Type;
		}

		public string GetDescription()
		{
			if (CustomDescription != null)
			{
				return "'" + CustomDescription + "'";
			}
			else
			{
				return Item.Description;
			}
		}

		public string ToFullString()
		{
			string res = ToString();

			if (!Item.IsCurrency())
			{
				res += " #" + InstanceID;
				if (OriginalInstanceID != InstanceID)
				{
					res += " [Formerly #" + OriginalInstanceID + "]";
				}
			}

			return res;
		}

		public ItemPriceInfo ToPriceInfo()
		{
			return new ItemPriceInfo(this);
		}

		public UnusualEffect GetUnusual()
		{
			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.UNUSUAL_ID);
			if (att == null)
			{
				return null;
			}

			return DataManager.Schema.Unusuals.FirstOrDefault((u) => u.ID == att.Value);
		}
		public int? GetCrateSeries()
		{
			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.CRATESERIES_ID);
			if (att == null)
			{
				return null;
			}

			return (int)att.Value;
		}

		public KillstreakType GetKillstreak()
		{
			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.KILLSTREAK_ID);
			if (att == null)
			{
				return KillstreakType.None;
			}

			return KillstreakTypes.Parse(att.Value.ToString("F0"));
		}

		public ItemPricing GetPriceSimple()
		{
			int? priceIndex = null;

			UnusualEffect ue = GetUnusual();
			if (ue != null)
			{
				priceIndex = ue.ID;
			}

			int? series = GetCrateSeries();
			if (series != null)
			{
				priceIndex = series.Value;
			}

			return DataManager.PriceData.GetPriceData(Item, Quality, priceIndex, Craftable, Tradable);
		}

		public SkinWear? GetSkinWear()
		{
			if (!Item.IsSkin())
			{
				return null;
			}

			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.SKINWEAR_ID);
			if (att == null)
			{
				return null;
			}

			return SkinWears.GetFromFloatingPoint(att.Value);
		}

		public bool IsAustralium()
		{
			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.AUSTRALIUM_ID);
			if (att == null)
			{
				return false;
			}

			return att.Value != 0;
		}
		
		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool shortened)
		{
			string result = TitleQuick(true);

			if (!shortened)
			{
				if (CustomName != null)
				{
					result += " (" + Item.ImproperName + ")";
				}

				if (!Tradable)
				{
					result = "Non-Tradable " + result;
				}
				if (!Craftable)
				{
					result = "Non-Craftable " + result;
				}
			}

			int? series = GetCrateSeries();
			if (series != null)
			{
				result += " No. " + series.Value;
			}
			
			if (!shortened && Count > 1)
			{
				result += " x" + Count;
			}

			return result;
		}
	}
}
