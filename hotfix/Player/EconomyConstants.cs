namespace Hotfix_LT.Player
{
	public struct EconomyConstants
	{
		public struct System
		{
			public const string EQUIPMENT = "Equipment";
			public const string GEM = "Gem";
			public const string GENERIC = "Generic";
			public const string SELECTBOX = "SelectBox";
			// public const string LUCKSTONE = "LuckStone";
			// public const string SCROLL = "Scroll";
			// public const string POTION = "Potion";
			// public const string CHESTS = "Chests";
			// public const string DARU = "Daru";
			// public const string KYANITE = "Kyanite";
			// public const string SOCKET_RODS = "Chisel";
		}

		public struct Quality
		{
			public const string LEGEND = "7";
			public const string HALLOWS = "6";
			public const string LEGENDARY = "5";
			public const string EPIC = "4";
			public const string UNCOMMON = "3";
			public const string COMMON = "2";
			public const string POOR = "1";
		}

		public struct EquipmentType
		{
			public const string MEDAL = "Medal";
			public const string WEAPON = "Weapon";
			public const string ARMOR = "Armor";
			public const string HEAD = "Head";
			public const string BROOCH = "Brooch";
			public const string WING = "Wing";
		}

		public static string AbToEquipmentType(string simple)
		{
			string type;
			switch (simple)
			{
				case "M":
					type = EquipmentType.MEDAL;
					break;
				case "W":
					type = EquipmentType.WEAPON;
					break;
				case "A":
					type = EquipmentType.ARMOR;
					break;
				case "H":
					type = EquipmentType.HEAD;
					break;
				case "B":
					type = EquipmentType.BROOCH;
					break;
				default:
					type = EquipmentType.MEDAL;
					EB.Debug.LogError("Equipment type not for simple={0}" , simple);
					break;
			}
			return type;
		}
	}
}