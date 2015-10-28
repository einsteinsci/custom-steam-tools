using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackpackTFPriceLister.Items
{
	public enum GunMettleCase
	{
		ConcealedKiller,
		Craftsmann,
		Teufort,
		Powerhouse
	}

	public enum SkinGrade
	{
		Civilian,
		Freelance,
		Mercenary,
		Commando,
		Assassin,
		Elite
	}

	public static class GunMettleSkins
	{
		public const string SCATTERGUN = "Scattergun";
		public const string PISTOL = "Pistol";
		public const string ROCKETLAUNCHER = "Rocket Launcher";
		public const string SHOTGUN = "Shotgun";
		public const string FLAMETHROWER = "Flame Thrower";
		public const string STICKYBOMBLAUNCHER = "Stickybomb Launcher";
		public const string MINIGUN = "Minigun";
		public const string MEDIGUN = "Medi Gun";
		public const string SNIPERRIFLE = "Sniper Rifle";
		public const string SMG = "SMG";
		public const string REVOLVER = "Revolver";

		public static List<Skin> Skins
		{
			get
			{
				if (_skins == null)
				{
					_skins = _initSkins();
				}

				return _skins;
			}
		}
		private static List<Skin> _skins;

		public static string ToReadableString(this GunMettleCase c)
		{
			switch (c)
			{
				case GunMettleCase.ConcealedKiller:
					return "Concealed Killer";
				default:
					return c.ToString();
			}
		}
		public static string ToReadableString(this SkinGrade g)
		{
			return g.ToString() + " Grade";
		}

		public static string GetPrefixString(this GunMettleCase c)
		{
			return c.ToString().ToLower() + "_";
		}

		public static GunMettleCase? ParseCaseNullable(string s)
		{
			for (GunMettleCase c = GunMettleCase.ConcealedKiller; c < GunMettleCase.Powerhouse; c++) // actually C#
			{
				if (c.ToReadableString().ToLower() == s.ToLower() ||
					c.ToString().ToLower() == s.ToLower() ||
					c.GetPrefixString() == s.ToLower())
				{
					return c;
				}
			}

			return null;
		}

		public static SkinGrade? ParseGradeNullable(string s)
		{
			for (SkinGrade g = SkinGrade.Civilian; g < SkinGrade.Elite; g++)
			{
				if (g.ToReadableString().ToLower() == s.ToLower() ||
					g.ToString().ToLower() == s.ToLower())
				{
					return g;
				}
			}

			return null;
		}

		private static List<Skin> _initSkins()
		{
			List<Skin> res = new List<Skin>();

			res.Add(new Skin("Sand Cannon", ROCKETLAUNCHER, GunMettleCase.ConcealedKiller, SkinGrade.Elite));
			res.Add(new Skin("Red Rock Roscoe", PISTOL, GunMettleCase.ConcealedKiller, SkinGrade.Elite));
			res.Add(new Skin("Purple Range", SNIPERRIFLE, GunMettleCase.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Psychedelic Slugger", REVOLVER, GunMettleCase.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Sudden Flurry", STICKYBOMBLAUNCHER, GunMettleCase.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Night Terror", SCATTERGUN, GunMettleCase.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Woodland Warrior", ROCKETLAUNCHER, GunMettleCase.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Carpet Bomber", STICKYBOMBLAUNCHER, GunMettleCase.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Wrapped Reviver", MEDIGUN, GunMettleCase.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Night Owl", SNIPERRIFLE, GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Woodsy Widowmaker", SMG, GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Backwoods Boomstick", SHOTGUN, GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("King of the Jungle", MINIGUN, GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Forest Fire", FLAMETHROWER , GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Masked Mender", MEDIGUN, GunMettleCase.ConcealedKiller, SkinGrade.Mercenary));

			res.Add(new Skin("Tartan Torpedo", SCATTERGUN, GunMettleCase.Craftsmann, SkinGrade.Commando));
			res.Add(new Skin("Lumber From Down Under", SNIPERRIFLE, GunMettleCase.Craftsmann, SkinGrade.Commando));
			res.Add(new Skin("Rustic Ruiner", SHOTGUN, GunMettleCase.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Barn Burner", FLAMETHROWER, GunMettleCase.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Homemade Heater", PISTOL, GunMettleCase.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Iron Wood", MINIGUN, GunMettleCase.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Country Crusher", SCATTERGUN, GunMettleCase.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Plaid Potshotter", SMG, GunMettleCase.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Shot in the Dark", SNIPERRIFLE, GunMettleCase.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Blasted Bombardier", STICKYBOMBLAUNCHER, GunMettleCase.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Reclaimed Reanimator", MEDIGUN, GunMettleCase.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Antique Annihilator", MINIGUN, GunMettleCase.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Old Country", REVOLVER, GunMettleCase.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("American Pastoral", ROCKETLAUNCHER, GunMettleCase.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Backcountry Blaster", SCATTERGUN, GunMettleCase.Craftsmann, SkinGrade.Civilian));

			res.Add(new Skin("Bovine Blazemaker", FLAMETHROWER, GunMettleCase.Teufort, SkinGrade.Commando));
			res.Add(new Skin("War Room", MINIGUN, GunMettleCase.Teufort, SkinGrade.Commando));
			res.Add(new Skin("Treadplate Tormenter", SMG, GunMettleCase.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Bogtrotter", SNIPERRIFLE, GunMettleCase.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Earth, Sky and Fire", FLAMETHROWER, GunMettleCase.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Team Sprayer", SMG, GunMettleCase.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Spruce Deuce", SCATTERGUN, GunMettleCase.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Hickory Hole-Puncher", PISTOL, GunMettleCase.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Rooftop Wrangler", STICKYBOMBLAUNCHER, GunMettleCase.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Civic Duty", SHOTGUN, GunMettleCase.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Civil Servant", MEDIGUN, GunMettleCase.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Local Hero", PISTOL, GunMettleCase.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Mayor", REVOLVER, GunMettleCase.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Smalltown Bringdown", ROCKETLAUNCHER, GunMettleCase.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Citizen Pain", MINIGUN, GunMettleCase.Teufort, SkinGrade.Civilian));

			res.Add(new Skin("Thunderbolt", SNIPERRIFLE, GunMettleCase.Powerhouse, SkinGrade.Elite));
			res.Add(new Skin("Liquid Asset", STICKYBOMBLAUNCHER, GunMettleCase.Powerhouse, SkinGrade.Elite));
			res.Add(new Skin("Shell Shocker", ROCKETLAUNCHER, GunMettleCase.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Current Event", SCATTERGUN, GunMettleCase.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Pink Elephant", STICKYBOMBLAUNCHER, GunMettleCase.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Flash Fryer", FLAMETHROWER, GunMettleCase.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Spark of Life", MEDIGUN, GunMettleCase.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Dead Reckoner", REVOLVER, GunMettleCase.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Black Dahlia", PISTOL, GunMettleCase.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Sandstone Special", PISTOL, GunMettleCase.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Lightning Rod", SHOTGUN, GunMettleCase.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Brick House", MINIGUN, GunMettleCase.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Aqua Marine", ROCKETLAUNCHER, GunMettleCase.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Low Profile", SMG, GunMettleCase.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Turbine Torcher", FLAMETHROWER, GunMettleCase.Powerhouse, SkinGrade.Mercenary));

			return res;
		}

		public static Skin GetSkin(string unlocalizedName)
		{
			return Skins.FirstOrDefault((s) => s.UnlocalizedName == unlocalizedName);
		}
	}
}
