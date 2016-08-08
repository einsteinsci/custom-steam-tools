using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSteamTools.Skins
{
	public enum CaseType
	{
		ConcealedKiller,
		Craftsmann,
		Teufort,
		Powerhouse,
		Harvest,
		Gentlemanne,
		Pyroland,
		Warbird
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

	public static class WeaponSkins
	{
		public const string SCATTERGUN = "Scattergun";
		public const string PISTOL = "Pistol";
		public const string ROCKETLAUNCHER = "Rocket Launcher";
		public const string SHOTGUN = "Shotgun";
		public const string FLAMETHROWER = "Flame Thrower";
		public const string GRENADELAUNCHER = "Grenade Launcher";
		public const string STICKYBOMBLAUNCHER = "Stickybomb Launcher";
		public const string MINIGUN = "Minigun";
		public const string WRENCH = "Wrench";
		public const string MEDIGUN = "Medi Gun";
		public const string SNIPERRIFLE = "Sniper Rifle";
		public const string SMG = "SMG";
		public const string REVOLVER = "Revolver";
		public const string KNIFE = "Knife";

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

		public static string ToReadableString(this CaseType c)
		{
			switch (c)
			{
				case CaseType.ConcealedKiller:
					return "Concealed Killer";
				default:
					return c.ToString();
			}
		}
		public static string ToReadableString(this SkinGrade g)
		{
			return g + " Grade";
		}
		public static ConsoleColor GetColor(this SkinGrade g)
		{
			switch (g)
			{
			case SkinGrade.Civilian:
				return ConsoleColor.White;
			case SkinGrade.Freelance:
				return ConsoleColor.DarkBlue;
			case SkinGrade.Mercenary:
				return ConsoleColor.Blue;
			case SkinGrade.Commando:
				return ConsoleColor.DarkMagenta;
			case SkinGrade.Assassin:
				return ConsoleColor.Magenta;
			case SkinGrade.Elite:
				return ConsoleColor.Red;
			default:
				return ConsoleColor.Gray;
			}
		}

		public static string GetPrefixString(this CaseType c)
		{
			return c.ToString().ToLower();
		}

		public static CaseType? ParseCaseNullable(string s)
		{
			for (CaseType c = CaseType.ConcealedKiller; c < CaseType.Powerhouse; c++) // actually C#
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

			// Gun Mettle //

			res.Add(new Skin("Sand Cannon", ROCKETLAUNCHER, CaseType.ConcealedKiller, SkinGrade.Elite));
			res.Add(new Skin("Red Rock Roscoe", PISTOL, CaseType.ConcealedKiller, SkinGrade.Elite));
			res.Add(new Skin("Purple Range", SNIPERRIFLE, CaseType.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Psychedelic Slugger", REVOLVER, CaseType.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Sudden Flurry", STICKYBOMBLAUNCHER, CaseType.ConcealedKiller, SkinGrade.Assassin));
			res.Add(new Skin("Night Terror", SCATTERGUN, CaseType.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Woodland Warrior", ROCKETLAUNCHER, CaseType.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Carpet Bomber", STICKYBOMBLAUNCHER, CaseType.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Wrapped Reviver", MEDIGUN, CaseType.ConcealedKiller, SkinGrade.Commando));
			res.Add(new Skin("Night Owl", SNIPERRIFLE, CaseType.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Woodsy Widowmaker", SMG, CaseType.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Backwoods Boomstick", SHOTGUN, CaseType.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("King of the Jungle", MINIGUN, CaseType.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Forest Fire", FLAMETHROWER , CaseType.ConcealedKiller, SkinGrade.Mercenary));
			res.Add(new Skin("Masked Mender", MEDIGUN, CaseType.ConcealedKiller, SkinGrade.Mercenary));

			res.Add(new Skin("Tartan Torpedo", SCATTERGUN, CaseType.Craftsmann, SkinGrade.Commando));
			res.Add(new Skin("Lumber From Down Under", SNIPERRIFLE, CaseType.Craftsmann, SkinGrade.Commando));
			res.Add(new Skin("Rustic Ruiner", SHOTGUN, CaseType.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Barn Burner", FLAMETHROWER, CaseType.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Homemade Heater", PISTOL, CaseType.Craftsmann, SkinGrade.Mercenary));
			res.Add(new Skin("Iron Wood", MINIGUN, CaseType.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Country Crusher", SCATTERGUN, CaseType.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Plaid Potshotter", SMG, CaseType.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Shot in the Dark", SNIPERRIFLE, CaseType.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Blasted Bombardier", STICKYBOMBLAUNCHER, CaseType.Craftsmann, SkinGrade.Freelance));
			res.Add(new Skin("Reclaimed Reanimator", MEDIGUN, CaseType.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Antique Annihilator", MINIGUN, CaseType.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Old Country", REVOLVER, CaseType.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("American Pastoral", ROCKETLAUNCHER, CaseType.Craftsmann, SkinGrade.Civilian));
			res.Add(new Skin("Backcountry Blaster", SCATTERGUN, CaseType.Craftsmann, SkinGrade.Civilian));

			res.Add(new Skin("Bovine Blazemaker", FLAMETHROWER, CaseType.Teufort, SkinGrade.Commando));
			res.Add(new Skin("War Room", MINIGUN, CaseType.Teufort, SkinGrade.Commando));
			res.Add(new Skin("Treadplate Tormenter", SMG, CaseType.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Bogtrotter", SNIPERRIFLE, CaseType.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Earth, Sky and Fire", FLAMETHROWER, CaseType.Teufort, SkinGrade.Mercenary));
			res.Add(new Skin("Team Sprayer", SMG, CaseType.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Spruce Deuce", SCATTERGUN, CaseType.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Hickory Hole-Puncher", PISTOL, CaseType.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Rooftop Wrangler", STICKYBOMBLAUNCHER, CaseType.Teufort, SkinGrade.Freelance));
			res.Add(new Skin("Civic Duty", SHOTGUN, CaseType.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Civil Servant", MEDIGUN, CaseType.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Local Hero", PISTOL, CaseType.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Mayor", REVOLVER, CaseType.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Smalltown Bringdown", ROCKETLAUNCHER, CaseType.Teufort, SkinGrade.Civilian));
			res.Add(new Skin("Citizen Pain", MINIGUN, CaseType.Teufort, SkinGrade.Civilian));

			res.Add(new Skin("Thunderbolt", SNIPERRIFLE, CaseType.Powerhouse, SkinGrade.Elite));
			res.Add(new Skin("Liquid Asset", STICKYBOMBLAUNCHER, CaseType.Powerhouse, SkinGrade.Elite));
			res.Add(new Skin("Shell Shocker", ROCKETLAUNCHER, CaseType.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Current Event", SCATTERGUN, CaseType.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Pink Elephant", STICKYBOMBLAUNCHER, CaseType.Powerhouse, SkinGrade.Assassin));
			res.Add(new Skin("Flash Fryer", FLAMETHROWER, CaseType.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Spark of Life", MEDIGUN, CaseType.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Dead Reckoner", REVOLVER, CaseType.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Black Dahlia", PISTOL, CaseType.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Sandstone Special", PISTOL, CaseType.Powerhouse, SkinGrade.Commando));
			res.Add(new Skin("Lightning Rod", SHOTGUN, CaseType.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Brick House", MINIGUN, CaseType.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Aqua Marine", ROCKETLAUNCHER, CaseType.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Low Profile", SMG, CaseType.Powerhouse, SkinGrade.Mercenary));
			res.Add(new Skin("Turbine Torcher", FLAMETHROWER, CaseType.Powerhouse, SkinGrade.Mercenary));

			// Tough Break //

			AddSkins(res, "Boneyard", CaseType.Harvest, SkinGrade.Commando, SNIPERRIFLE, WRENCH, REVOLVER, KNIFE);
			AddSkins(res, "Pumpkin Patch", CaseType.Harvest, SkinGrade.Mercenary, 
				FLAMETHROWER, MINIGUN, SNIPERRIFLE, STICKYBOMBLAUNCHER);
			AddSkins(res, "Macabre Web", CaseType.Harvest, SkinGrade.Freelance,
				PISTOL, REVOLVER, SCATTERGUN, GRENADELAUNCHER, STICKYBOMBLAUNCHER, MINIGUN);
			AddSkins(res, "Autumn", CaseType.Harvest, SkinGrade.Freelance, 
				FLAMETHROWER, GRENADELAUNCHER, STICKYBOMBLAUNCHER);
			AddSkins(res, "Autumn", CaseType.Harvest, SkinGrade.Civilian, ROCKETLAUNCHER, SHOTGUN, WRENCH);
			AddSkins(res, "Nutcracker", CaseType.Harvest, SkinGrade.Civilian,
				FLAMETHROWER, MINIGUN, PISTOL, SCATTERGUN, WRENCH);
			AddSkins(res, "Wildwood", CaseType.Harvest, SkinGrade.Civilian, MEDIGUN, REVOLVER, SMG, SNIPERRIFLE);

			AddSkins(res, "Top Shelf", CaseType.Gentlemanne, SkinGrade.Commando, MINIGUN, WRENCH, GRENADELAUNCHER);
			AddSkins(res, "Top Shelf", CaseType.Gentlemanne, SkinGrade.Mercenary, KNIFE, REVOLVER);
			AddSkins(res, "High Roller's", CaseType.Gentlemanne, SkinGrade.Mercenary, MEDIGUN, SMG);
			AddSkins(res, "Coffin Nail", CaseType.Gentlemanne, SkinGrade.Freelance, 
				ROCKETLAUNCHER, SCATTERGUN, SHOTGUN, GRENADELAUNCHER);
			AddSkins(res, "High Roller's", CaseType.Gentlemanne, SkinGrade.Freelance, ROCKETLAUNCHER);
			AddSkins(res, "Coffin Nail", CaseType.Gentlemanne, SkinGrade.Civilian,
				FLAMETHROWER, MEDIGUN, MINIGUN, REVOLVER, SNIPERRIFLE, STICKYBOMBLAUNCHER);
			AddSkins(res, "Dressed to Kill", CaseType.Gentlemanne, SkinGrade.Civilian,
				KNIFE, MEDIGUN, MINIGUN, PISTOL, SHOTGUN, SNIPERRIFLE, STICKYBOMBLAUNCHER, WRENCH);

			AddSkins(res, "Rainbow", CaseType.Pyroland, SkinGrade.Elite, GRENADELAUNCHER, SNIPERRIFLE, FLAMETHROWER);
			AddSkins(res, "Balloonicorn", CaseType.Pyroland, SkinGrade.Assassin, SNIPERRIFLE, FLAMETHROWER);
			AddSkins(res, "Sweet Dreams", CaseType.Pyroland, SkinGrade.Assassin, GRENADELAUNCHER, STICKYBOMBLAUNCHER);
			AddSkins(res, "Mister Cuddles", CaseType.Pyroland, SkinGrade.Commando, MINIGUN);
			AddSkins(res, "Blue Mew", CaseType.Pyroland, SkinGrade.Commando, KNIFE, PISTOL, ROCKETLAUNCHER, SCATTERGUN);
			AddSkins(res, "Shot to Hell", CaseType.Pyroland, SkinGrade.Commando, SCATTERGUN, PISTOL);
			AddSkins(res, "Torqued to Hell", CaseType.Pyroland, SkinGrade.Commando, WRENCH);
			AddSkins(res, "Blue Mew", CaseType.Pyroland, SkinGrade.Mercenary, SMG);
			AddSkins(res, "Stabbed to Hell", CaseType.Pyroland, SkinGrade.Mercenary, KNIFE);
			AddSkins(res, "Brain Candy", CaseType.Pyroland, SkinGrade.Mercenary, KNIFE, MINIGUN, PISTOL, ROCKETLAUNCHER);
			AddSkins(res, "Flower Power", CaseType.Pyroland, SkinGrade.Mercenary, MEDIGUN, REVOLVER, SCATTERGUN, SHOTGUN);

			AddSkins(res, "Killer Bee", CaseType.Warbird, SkinGrade.Elite, SCATTERGUN);
			AddSkins(res, "Warhawk", CaseType.Warbird, SkinGrade.Elite, ROCKETLAUNCHER);
			AddSkins(res, "Warhawk", CaseType.Warbird, SkinGrade.Assassin, GRENADELAUNCHER, FLAMETHROWER);
			AddSkins(res, "Red Bear", CaseType.Warbird, SkinGrade.Assassin, SHOTGUN);
			AddSkins(res, "Butcher Bird", CaseType.Warbird, SkinGrade.Commando, MINIGUN);
			AddSkins(res, "Airwolf", CaseType.Warbird, SkinGrade.Commando, SNIPERRIFLE, KNIFE);
			AddSkins(res, "Blitzkrieg", CaseType.Warbird, SkinGrade.Commando, STICKYBOMBLAUNCHER);
			AddSkins(res, "Corsair", CaseType.Warbird, SkinGrade.Commando, MEDIGUN);
			AddSkins(res, "Blitzkrieg", CaseType.Warbird, SkinGrade.Mercenary, MEDIGUN, PISTOL, REVOLVER, SMG, KNIFE);
			AddSkins(res, "Airwolf", CaseType.Warbird, SkinGrade.Mercenary, WRENCH);
			AddSkins(res, "Corsair", CaseType.Warbird, SkinGrade.Mercenary, SCATTERGUN);
			AddSkins(res, "Butcher Bird", CaseType.Warbird, SkinGrade.Mercenary, GRENADELAUNCHER);

			return res;
		}

		public static void AddSkins(List<Skin> skins, string name, CaseType _case, SkinGrade grade, params string[] weapons)
		{
			skins.AddRange(weapons.Select(w => new Skin(name, w, _case, grade)));
		}

		public static Skin GetSkin(string unlocalizedName)
		{
			return Skins.FirstOrDefault((s) => s.UnlocalizedName == unlocalizedName);
		}
	}
}
