using BackpackTFPriceLister.Json.BackpackDataJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister
{
	public class ItemInstance
	{
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

		public ItemInstance(ItemInstanceJson json, TF2Data reference)
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

			Attributes = new List<AppliedInstanceAttribute>();
		}

		public string TitleQuick()
		{
			if (CustomName != null)
			{
				return "'" + CustomName + "'";
			}

			string q = Quality.ToReadableString();
			if (q != "")
			{
				return q + " " + Item.Name;
			}

			return Item.Name;
		}

		public string GetSubtext()
		{
			return "Level " + Level.ToString() + " " + Item.Type;
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
				res += " #" + InstanceID.ToString();
				if (OriginalInstanceID != InstanceID)
				{
					res += " [Formerly #" + OriginalInstanceID.ToString() + "]";
				}
			}

			return res;
		}

		public UnusualEffect GetUnusual()
		{
			AppliedInstanceAttribute att = Attributes.FirstOrDefault((a) => a.ID == ItemAttribute.UNUSUAL_ID);
			if (att == null)
			{
				return null;
			}

			return DataManager.ItemData.Unusuals.FirstOrDefault((u) => u.ID == att.Value);
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

		public ItemPricing GetPrice()
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

		public override string ToString()
		{
			string result = TitleQuick();
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

			int? series = GetCrateSeries();
			if (series != null)
			{
				result += " No. " + series.Value.ToString();
			}

			if (Count != 1)
			{
				result += " x" + Count.ToString();
			}

			return result;
		}
	}
}
