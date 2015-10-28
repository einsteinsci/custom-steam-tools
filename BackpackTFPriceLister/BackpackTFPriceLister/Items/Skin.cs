using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackpackTFPriceLister.Lookup;

namespace BackpackTFPriceLister.Items
{
	public class Skin
	{
		public string Name
		{ get; private set; }

		public string UnlocalizedName
		{ get; private set; }

		public GunMettleCase Collection
		{ get; private set; }

		public SkinGrade Grade
		{ get; private set; }

		public string BaseWeapon
		{ get; private set; }

		public string Description => Grade.ToReadableString() + " " + BaseWeapon;

		public Skin(string name, string weapon, GunMettleCase collection, SkinGrade grade)
		{
			Name = name;
			BaseWeapon = weapon;
			Collection = collection;
			Grade = grade;

			string weaponDesc = weapon.ToLower().Replace(" ", "");

			UnlocalizedName = collection.GetPrefixString() + weaponDesc +
				Name.ToLower().Replace(" ", "").Replace(",", "").Replace("-", "");
		}

		public string GetMarketHash(SkinWear? wear)
		{
			if (wear == null)
			{
				return Name + " " + BaseWeapon;
			}

			return Name + " " + BaseWeapon + " " + wear.Value.WithParentheses();
		}

		public Item GetItemForm(TF2Data data)
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
			return Name + " (" + Collection.ToString() + ")";
		}
	}
}
