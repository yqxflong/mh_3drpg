///////////////////////////////////////////////////////////////////////
//
//  SparxSocketData.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
	public class SocketData : EB.Replication.ISerializable
	{
		public class SpawnerData : EB.Replication.ISerializable
		{
			private int _spawnerIndex;
			public int SpawnerIndex { get { return _spawnerIndex; } set { _spawnerIndex = value; } }
			public List<CharacterData> SpawnedCharacters { get; set; }

			public void Serialize(EB.BitStream bs)
			{
				bs.Serialize(ref _spawnerIndex);

				if (bs.isWriting)
				{
					int numCharacterData = SpawnedCharacters.Count;
					bs.Serialize(ref numCharacterData);
					foreach (CharacterData character in SpawnedCharacters)
					{
						character.Serialize(bs);
					}
				}
				else
				{
					int numCharacterData = 0;
					bs.Serialize(ref numCharacterData);
					SpawnedCharacters = new List<CharacterData>();
					for (int i = 0; i < numCharacterData; i++)
					{
						CharacterData c = new CharacterData();
						c.Serialize(bs);
						SpawnedCharacters.Add(c);
					}
				}
			}
		}

		public class LootableData : EB.Replication.ISerializable
		{
			private int _lootableIndex;
			public int LootableIndex { get { return _lootableIndex; } set { _lootableIndex = value; } }
			public DropGroupData DropGroup { get; set; }

			public List<DropData> GetDropsForPlayer(string playerId)
			{
				if (DropGroup.Drops.ContainsKey(playerId))
				{
					return DropGroup.Drops[playerId];
				}
				return new List<DropData>();
			}

			public void Serialize(EB.BitStream bs)
			{
				bs.Serialize(ref _lootableIndex);

				if (bs.isReading)
				{
					DropGroup = new DropGroupData();
				}
				DropGroup.Serialize(bs);
			}
		}

		public class CharacterData : EB.Replication.ISerializable
		{
			private int _locationIndex;
			public int LocationIndex { get { return _locationIndex; } set { _locationIndex = value; } }
			private int _characterId;
			public int CharacterId { get { return _characterId; } set { _characterId = value; } }
			public DropGroupData DropGroup { get; set; }
			private int _xp;
			public int XP { get { return _xp; } set { _xp = value; }}

			public List<DropData> GetDropsForPlayer(string playerId)
			{
				if (DropGroup.Drops.ContainsKey(playerId))
				{
					return DropGroup.Drops[playerId];
				}
				return new List<DropData>();
			}

			public List<CharacterData> SplitNWays(int n)
			{
				List<CharacterData> newData = new List<CharacterData>();
				for (int i = 0; i < n; i++)
				{
					CharacterData newCharData = new CharacterData();
					newCharData.DropGroup = new DropGroupData();
					newCharData.XP = (int)Mathf.Floor((float)XP / n);
					newData.Add(newCharData);
				}

				foreach (string key in DropGroup.Drops.Keys)
				{
					List<DropData> dropsForPlayer = null;
					if (DropGroup.Drops.ContainsKey(key))
					{
						dropsForPlayer = DropGroup.Drops[key];
					} 
					else
					{
						dropsForPlayer = new List<DropData>();
					}
					
					for (int i = 0; i < dropsForPlayer.Count; i++)
					{
						Dictionary<string, List<DropData>> splitDropsForPlayer = newData[i % n].DropGroup.Drops;
						List<DropData> dropList = splitDropsForPlayer.ContainsKey(key) ? splitDropsForPlayer[key] : new List<DropData>();
						if (dropList == null)
						{
							dropList = new List<DropData>();
						}
						dropList.Add(dropsForPlayer[i]);
						splitDropsForPlayer[key] = dropList;
					}	
				}

				return newData;
			}

			public void Serialize(EB.BitStream bs)
			{
				bs.Serialize(ref _locationIndex);
				bs.Serialize(ref _characterId);
				bs.Serialize(ref _xp);

				if (bs.isReading)
				{
					DropGroup = new DropGroupData();
				}
				DropGroup.Serialize(bs);
			}
		}

		public class DropGroupData : EB.Replication.ISerializable
		{
			public Dictionary<string, List<DropData>> Drops { get; set; }

			public DropGroupData()
			{
				Drops = new Dictionary<string, List<DropData>>();
			}

			public void Serialize(EB.BitStream bs)
			{
				if (bs.isWriting)
				{
					int numDrops = Drops.Count;
					bs.Serialize(ref numDrops);
					foreach (KeyValuePair<string, List<DropData>> pair in Drops)
					{
						int numData = pair.Value.Count;
						bs.Serialize(ref numData);

						string key = pair.Key;
						bs.Serialize(ref key);

						foreach (DropData drop in pair.Value)
						{
							drop.Serialize(bs);
						}
					}
				}
				else
				{
					int numDrops = 0;
					bs.Serialize(ref numDrops);
					Drops = new Dictionary<string, List<DropData>>();
					for (int i = 0; i < numDrops; i++)
					{
						int numData = 0;
						bs.Serialize(ref numData);

						string key = string.Empty;
						bs.Serialize(ref key);

						List<DropData> dropList = new List<DropData>();
						for (int j = 0; j < numData; j++)
						{
							DropData d = new DropData();
							d.Serialize(bs);
							dropList.Add(d);
						}

						Drops[key] = dropList;
					}
				}
			}
		}

		public class DropData : EB.Replication.ISerializable
		{
			private int _itemId;
			public int ItemId { get { return _itemId; } set { _itemId = value; } }
			private int _rarity;
			public int Rarity { get { return _rarity; } set { _rarity = value; } }
			private int _spiritId;
			public int SpiritId { get { return _spiritId; } set { _spiritId = value; } }
			private int _gold;
			public int Gold { get { return _gold; } set { _gold = value; } }
			private int _itemLevel;
			public int ItemLevel { get { return _itemLevel; } set { _itemLevel = value; } }
			public List<int> AffixIds { get; set; }

			public void Serialize(EB.BitStream bs)
			{
				bs.Serialize(ref _itemId);
				bs.Serialize(ref _itemLevel);
				bs.Serialize(ref _rarity);
				bs.Serialize(ref _spiritId);
				bs.Serialize(ref _gold);

				if (bs.isWriting)
				{
					int numAffixIds = AffixIds.Count;
					bs.Serialize(ref numAffixIds);
					foreach (int affixId in AffixIds)
					{
						int aId = affixId;
						bs.Serialize(ref aId);
					}
				}
				else
				{
					int numAffixIds = 0;
					bs.Serialize(ref numAffixIds);
					AffixIds = new List<int>();
					for (int i = 0; i < numAffixIds; i++)
					{
						int aId = 0;
						bs.Serialize(ref aId);
						AffixIds.Add(aId);
					}
				}
			}
		}

		private float _posX;
		public float PositionX 
		{ 
			get
			{
				return _posX;
			}
			set
			{
				_posX = value;
			}
		}
		private float _posZ;
		public float PositionZ
		{ 
			get
			{
				return _posZ;
			}
			set
			{
				_posZ = value;
			}
		}
		private string _selectedZone;
		public string SelectedZone
		{
			get
			{
				return _selectedZone;
			}
			set
			{
				_selectedZone = value;
			}
		}
		private int _interactionSetIndex;
		public int InteractionSetIndex
		{
			get
			{
				return _interactionSetIndex;
			}
			set
			{
				_interactionSetIndex = value;
			}
		}
		private string _layoutName;
		public string LayoutName
		{
			get
			{
				return _layoutName;
			}
			set
			{
				_layoutName = value;
			}
		}

		public List<string> Tags { get; set; }
		public List<SpawnerData> Spawners { get; set; }
		public List<LootableData> Lootables { get; set; }
		
		public SocketData( float positionX, float positionZ, List<string> tags, string selectedZone, int interactionSetIndex )
		{
			PositionX = positionX;
			PositionZ = positionZ;
			Tags = tags;
			SelectedZone = selectedZone;
			InteractionSetIndex = interactionSetIndex;
		}

		public SocketData() : this(0, 0, new List<string>(), string.Empty, 0)
		{
		}

		public void Serialize(EB.BitStream bs)
		{
			bs.Serialize(ref _posX);
			bs.Serialize(ref _posZ);
			bs.Serialize(ref _selectedZone);
			bs.Serialize(ref _interactionSetIndex);
			bs.Serialize(ref _layoutName);

			if (bs.isWriting)
			{
				int numTags = Tags.Count;
				bs.Serialize(ref numTags);
				foreach (string tag in Tags)
				{
					string t = tag;
					bs.Serialize(ref t);
				}

				int numSpawners = Spawners.Count;
				bs.Serialize(ref numSpawners);
				foreach (SpawnerData spawner in Spawners)
				{
					spawner.Serialize(bs);
				}

				int numLootables = Lootables.Count;
				bs.Serialize(ref numLootables);
				foreach (LootableData lootable in Lootables)
				{
					lootable.Serialize(bs);
				}
			}
			else
			{
				int numTags = 0;
				bs.Serialize(ref numTags);
				Tags = new List<string>();
				for (int i = 0; i < numTags; i++)
				{
					string tag = string.Empty;
					bs.Serialize(ref tag);
					Tags.Add(tag);
				}

				int numSpawners = 0;
				bs.Serialize(ref numSpawners);
				Spawners = new List<SpawnerData>();
				for (int i = 0; i < numSpawners; i++)
				{
					SpawnerData spawner = new SpawnerData();
					spawner.Serialize(bs);
					Spawners.Add(spawner);
				}

				int numLootables = 0;
				bs.Serialize(ref numLootables);
				Lootables = new List<LootableData>();
				for (int i = 0; i < numLootables; i++)
				{
					LootableData lootable = new LootableData();
					lootable.Serialize(bs);
					Lootables.Add(lootable);
				}
			}
		}
	}

	public class SocketDataSet : EB.Replication.ISerializable
	{
		public List<SocketData> socketData;
		private string _hostInstanceId;

		public string HostInstanceId
		{
			get
			{
				return _hostInstanceId;
			}
			set
			{
				_hostInstanceId = value;
			}
		}

		public SocketDataSet()
		{
			socketData = new List<SocketData>();
		}

		public void Serialize(EB.BitStream bs)
		{
			bs.Serialize(ref _hostInstanceId);

			if (bs.isWriting)
			{
				int numSocketData = socketData.Count;
				bs.Serialize(ref numSocketData);
				foreach(SocketData socket in socketData)
				{
					socket.Serialize(bs);
				}
			}
			else
			{
				int numSocketData = 0;
				bs.Serialize(ref numSocketData);
				socketData = new List<SocketData>();
				for (int i = 0; i < numSocketData; i++)
				{
					SocketData socket = new SocketData();
					socket.Serialize(bs);
					socketData.Add(socket);
				}
			}
		}
	}
}
