using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSteamTools.Lookup;
using CustomSteamTools.Schema;

using UltimateUtil;

namespace CustomSteamTools.Skins
{
	public class Skin
	{
		public string Name
		{ get; private set; }

		public string UnlocalizedName
		{ get; private set; }

		public CaseType Collection
		{ get; private set; }

		public SkinGrade Grade
		{ get; private set; }

		public string BaseWeapon
		{ get; private set; }

		public string Description => Grade.ToReadableString() + " " + BaseWeapon;

		public Skin(string name, string weapon, CaseType collection, SkinGrade grade)
		{
			Name = name;
			BaseWeapon = weapon;
			Collection = collection;
			Grade = grade;

			string weaponDesc = weapon.ToLower().Replace(" ", "");

			UnlocalizedName = collection.GetPrefixString() + "_" + weaponDesc + "_" +
				Name.ToLower().RemoveChars(' ', ',', '-', '\'');
		}

		public string GetMarketHash(SkinWear? wear)
		{
			if (wear == null)
			{
				return Name + " " + BaseWeapon;
			}

			return Name + " " + BaseWeapon + " " + wear.Value.WithParentheses();
		}

		public Item GetItemForm(GameSchema data)
		{
			foreach (Item i in data.Items)
			{
				if (i.UnlocalizedName == UnlocalizedName)
				{
					return i;
				}
			}

			return null;
		}

		public override string ToString()
		{
			return Name + " " + BaseWeapon + " (" + Collection + ")";
		}
	}
}
