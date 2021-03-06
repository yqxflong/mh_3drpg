using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class LevelUpNode
	{
		public LevelUpNode( Hashtable data )
		{
			this.NewLevel = EB.Dot.Integer( "newLevel", data, 0 );
			this.Category = EB.Dot.String( "category", data, string.Empty );
			this.Rewards = EB.Dot.List< RedeemerItem >( "prizes", data, null );
			
			if( this.Rewards == null )
			{
				this.Rewards = new List<RedeemerItem>();
			}
		}
		
		public List<RedeemerItem> Rewards { get; private set; }
		public int NewLevel { get; private set; }
		public string Category { get; private set; }
	}
	
	public class LevelUpComparer: IComparer<LevelUpNode>
	{
		public int Compare(LevelUpNode a, LevelUpNode b)
		{
			if (a.NewLevel > b.NewLevel)
			{
				return 1;
			}
			else if (a.NewLevel < b.NewLevel)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}
	
	public class LevelRewardsType
	{
		public LevelRewardsType( string name, int levelNum, int nextThreshold, int prevThreshold )
		{
			this.Name = name;
			this.Level = levelNum;
			this.NextResourceThreshold = nextThreshold;
			this.PreviousResourceThreshold = prevThreshold;
			//EB.Debug.Log("LevelRewardsType name=" + name + " levelNum=" + levelNum + " nextThreshold=" + nextThreshold + " prevThreshold=" + prevThreshold);
		}
		
		public string Name { get; private set; }
		public int Level { get; private set; }
		public long NextResourceThreshold { get; private set; }
		public long PreviousResourceThreshold { get; private set; }
	}	

	public class LevelRewardsStatus
	{
		public LevelRewardsStatus( Hashtable data = null )
		{
			this.IsEnabled = false;
			LevelRewardsStatus.DefaultCategory = "xp";

			if( data != null )
			{
				this.Types = new List<LevelRewardsType>();
			
				Hashtable levelData = EB.Dot.Object( LevelRewardsStatus.DefaultCategory, data, null );
				//LevelRewardsManager.PrintHashTable("levelData", levelData);
				this.IsEnabled = true;
				this.Level = EB.Dot.Integer( "last_awarded_level", levelData, 0 );
				
				foreach( DictionaryEntry entry in data )
				{
					string name = entry.Key.ToString();
					levelData = EB.Dot.Object( name, data, null );
					int levelNum = EB.Dot.Integer("last_awarded_level", levelData, 0);
					int nextThreshold = EB.Dot.Integer("nextLevelXp", levelData, 0);
					int prevThreshold = EB.Dot.Integer("prevLevelXp", levelData, 0);
					
					this.Types.Add( new LevelRewardsType(name, levelNum, nextThreshold, prevThreshold) );
				}
			}
		}
		
		public int GetLevel(string typeName)
		{
			if (this.Types != null)
			{
				foreach (LevelRewardsType entry in this.Types) 
				{
					if (entry.Name == typeName)
					{
						return entry.Level;
					}
				}
			}
			return 0;
		}

		public int Level { get; private set; }
		public bool IsEnabled { get; private set; }
		static public string DefaultCategory { get; private set; }
		
		public List<LevelRewardsType> Types { get; private set; }
		
	}
	
	public class LevelMilestone
	{
		public LevelMilestone( Hashtable data = null )
		{	
			if( data != null )
			{
				this.tag = EB.Dot.String( "tag", data, string.Empty );
				this.category = EB.Dot.String( "category", data, string.Empty );
				this.level = EB.Dot.Integer( "level_num", data, 0 );

				Hashtable redeemerData = EB.Dot.Object("redeemer", data, null);
				if( redeemerData != null )
				{
					this.redeemer = new Sparx.RedeemerItem( redeemerData );
				}
			}
		}
		
		public string tag;
		public string category;
		public int level;
		public RedeemerItem redeemer;
	}
	
	public class LevelMilestoneStatus
	{
		public LevelMilestoneStatus( ArrayList data = null )
		{	
			this.milestones = new List<LevelMilestone>();
		
			if (data != null)
			{
				for (int i=0; i<data.Count; i++)
				{
					this.milestones.Add( new LevelMilestone( (Hashtable)data[i]) );
				}
			}
		}
		
		public List<LevelMilestone> milestones { get; private set; }
	}

	public class LevelRewardsManager : SubSystem, Updatable
	{
		LevelRewardsAPI _api = null;
		LevelRewardsStatus _status = new LevelRewardsStatus();
		LevelMilestoneStatus _milestones = new LevelMilestoneStatus();

		EB.SafeInt				_level;
		//EB.SafeLong				_xpAmount;

		public delegate void LevelRewardsChangeDel(LevelRewardsStatus status);
		public LevelRewardsChangeDel OnLevelChange;
		
		public LevelRewardsStatus Status { get { return _status; }	}

        public int CurLevel;
		public int Level { get { return _level; } }
        public bool IsLevelUp
        {
            get
            {
                foreach (LevelUpNode node in LevelUpQueue)
                {
                    if (node.Category == "xp")
                    {
                        return (Level > CurLevel);
                    }
                }
                return false;
            }
        }

		public List<LevelUpNode> LevelUpQueue { get; private set; }
		
		public LevelMilestoneStatus MilestoneStatus { get { return _milestones; } }
		
		void OnFetch( string err, Hashtable data )
		{
			if( string.IsNullOrEmpty( err ) == true )
			{
				Hashtable levelData = EB.Dot.Object( "levelrewards", data, null );
				if (levelData != null)
				{	
					OnLevelData( levelData );
				}
				
				ArrayList levelMilestoneData = EB.Dot.Array( "levelrewards_milestones", data, null);
				if(levelMilestoneData == null)
				{
					this._milestones = new LevelMilestoneStatus();
				}
				else
				{
					this._milestones = new LevelMilestoneStatus(levelMilestoneData);
				}
			}
			else
			{
				EB.Debug.LogError( "Error Fetching Level Rewards: {0}", err );
			}
		}
		
		void OnLevelData( Hashtable data )
		{
			//PrintHashTable("levelrewards", data);
			this._status = new LevelRewardsStatus( data );
			
			_level = this._status.Level;
			
			if (OnLevelChange != null) { OnLevelChange(_status); }
		}

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            _level = CurLevel = 0;
        }

        void OnLevelUp( Hashtable data )
		{
			//PrintHashTable("OnLevelUp", data);
			QueueLevelUp( data );
			
			Hashtable levelData = EB.Dot.Object( "levelrewards", data, null );
			if (levelData != null)
			{	
				OnLevelData( levelData );
			}
			else
			{
				if (OnLevelChange != null) { OnLevelChange(_status); }
			}
		}
		
		void QueueLevelUp( Hashtable data )
		{
			LevelUpNode node = new LevelUpNode( data );
			
			LevelUpQueue.Add(node);
			
			LevelUpComparer comp = new LevelUpComparer();
			LevelUpQueue.Sort(comp);
		}
		
		public LevelUpNode GetNextLevelUp(string typeName)
		{
			LevelUpNode levelUp = null;
			
			foreach(LevelUpNode node in LevelUpQueue)
			{
				if (node.Category == typeName)
				{
					levelUp = node;
					LevelUpQueue.Remove(node);
					break;
				}
			}
			return levelUp;
		}

		public LevelUpNode GetNextLevelUpNotMove(string typeName)
		{
			LevelUpNode levelUp = null;

			foreach (LevelUpNode node in LevelUpQueue)
			{
				if (node.Category == typeName)
				{
					levelUp = node;
					break;
				}
			}

			return levelUp;
		}

        public int[] GetLevelupXpInfo()
        {
            int[] temp = new int[2];
            bool isFirst = true;
            for (int i=0;i< LevelUpQueue.Count;i++)
            {
                if (LevelUpQueue[i].Category == "xp")
                {
                    if (isFirst )
                    {
                        CurLevel = LevelUpQueue[i].NewLevel - 1;
                        temp[0] = LevelUpQueue[i].Rewards[0].Balance- LevelUpQueue[i].Rewards[0].Quantity ;
                        isFirst = false;
                    }
                    temp[1] = LevelUpQueue[i].Rewards[0].Balance;
                }
            }
            LevelUpQueue.Clear();
            return temp;
        }

		public int GetLevel(string typeName)
		{
			return this._status.GetLevel(typeName);
		}
		
		public int GetMilestoneRedeemerLevel(string redeemerType, int quanity, string category = null)
		{
			if (category == null)
			{
				category = LevelRewardsStatus.DefaultCategory;
			}
		
			if (this._milestones.milestones != null)
			{
				for (int i=0; i<this._milestones.milestones.Count; i++)
				{
					LevelMilestone m = this._milestones.milestones[i];
					if (m.redeemer != null)
					{
						if ((m.category == category) && (m.redeemer.Type == redeemerType) && (m.redeemer.Quantity == quanity))
						{
							return m.level;
						}
					}
				}
			}
			return -1;
		}
		
		public int GetMilestoneRedeemerLevel(string redeemerType, string dataType, int quanity, string category = null)
		{
			if (category == null)
			{
				category = LevelRewardsStatus.DefaultCategory;
			}
		
			if (this._milestones.milestones != null)
			{
				for (int i=0; i<this._milestones.milestones.Count; i++)
				{
					LevelMilestone m = this._milestones.milestones[i];
					if (m.redeemer != null)
					{
						if ((m.category == category) && (m.redeemer.Type == redeemerType) && (m.redeemer.Data == dataType) && (m.redeemer.Quantity == quanity))
						{
							return m.level;
						}
					}
				}
			}
			return -1;
		}
		
		public int GetMilestoneRedeemerLevel(string redeemerType, string dataType, string category = null)
		{
			if (category == null)
			{
				category = LevelRewardsStatus.DefaultCategory;
			}
		
			if (this._milestones.milestones != null)
			{
				for (int i=0; i<this._milestones.milestones.Count; i++)
				{
					LevelMilestone m = this._milestones.milestones[i];
					if (m.redeemer != null)
					{
						if ((m.category == category) && (m.redeemer.Type == redeemerType) && (m.redeemer.Data == dataType))
						{
							return m.level;
						}
					}
				}
			}
			return -1;
		}

		public static void PrintHashTable(string title, Hashtable data)
		{
			EB.Debug.Log("******[LevelRewardsManager] " + title + " data.Count=" + data.Count);
			foreach( DictionaryEntry entry in data)
			{
				EB.Debug.Log("      Entry: " + entry.Key.ToString() + " : " + entry.Value.ToString());
			}
		}

		
		#region implemented abstract members of EB.Sparx.Manager
		public override void Initialize( Config config )
		{
			_api = new LevelRewardsAPI(Hub.ApiEndPoint);
            LevelUpQueue = new List<LevelUpNode>();
		}
		
		public bool UpdateOffline { get { return false;} }
		
		public override void Connect()
		{
			var levelRewardsData = Dot.Object( "levelrewards", Hub.DataStore.LoginDataStore.LoginData, null );
			if( levelRewardsData != null )
			{
				this.OnFetch( null, Hub.DataStore.LoginDataStore.LoginData);
			}

			State = SubSystemState.Connected;
		}
		
		public void Update ()
		{
		}
		
		public override void Disconnect (bool isLogout)
		{
		}
		
		
		public override void Async (string message, object payload)
		{
			switch(message.ToLower())
			{
				case "level-up":
				{
					Hashtable data = payload as Hashtable;
					if( data == null )
					{
						data = JSON.Parse(payload.ToString()) as Hashtable;
					}
					
					if( data != null )
					{
						OnLevelUp( data );
					}
					break;
				}
				case "sync":
				{
					this._api.FetchStatus( OnFetch );
					break;
				}
				default:{
					break;
				}
			}
		}
		#endregion
	}
}

