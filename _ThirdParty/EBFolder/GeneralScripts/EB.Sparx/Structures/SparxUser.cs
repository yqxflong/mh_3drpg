using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class User
	{
		public Id Id {get;private set;}
		public string Name {get;private set;}
		public bool HasName {get{return !string.IsNullOrEmpty(Name);}}
		
		public string Email {get;private set;}
		public bool HasEmail {get{return !string.IsNullOrEmpty(Email);}}
		
		public string FacebookId {get;private set;}
		public bool HasFacebookId {get{return !string.IsNullOrEmpty(FacebookId);}}
		
		public bool IsGuest { get{ return !HasEmail; } }
		
		public string GameCenterId {get;private set;}
		
		public int CohortDate  {get;set;}
		
		public int Revenue { get; set; }

		public int WorldId { get; set; }

		public int RealmId { get; set; }

		public int Level { get; set; }

		public string Icon { get; set; }

		public int CharacterId { get; set; }

		public int Vip { get; set; }

		public int CreateTime { get; set; }
		
		public User(Id id)
		{
			Id = id;
			Name = string.Empty;
			Email = string.Empty;
			GameCenterId = string.Empty;
		}
		
		public void Update( object data )
		{
			Name		= Dot.String("name", data, Name);
			Email 		= Dot.String("email", data, Email);
			GameCenterId= Dot.String("gcid", data, GameCenterId);
			FacebookId  = Dot.String("fbid", data, FacebookId);
			CohortDate	= Dot.Integer("cohort", data, CohortDate);
			Revenue		= Dot.Integer("revenue", data, Revenue);
			WorldId		= Dot.Integer("wid", data, WorldId);
			RealmId		= Dot.Integer("rid", data, RealmId);
			Level		= Dot.Integer("level", data, Level);
			Icon		= Dot.String("icon", data, Icon);
			CharacterId = Dot.Integer("charid", data, CharacterId);
			Vip			= Dot.Integer("vip", data, Vip);
			CreateTime  = Dot.Integer("time_join", data, CreateTime);
		}
		
		Hashtable ToHashtable()
		{
			Hashtable data = Johny.HashtablePool.Claim();
			data["uid"] = Id;
			data["name"] = Name;
			return data;
		}
		
		public void Load( string key )  
		{
			Update( SecurePrefs.GetJSON(key) ); 
		}
		
		public void Save( string key )
		{
			SecurePrefs.SetJSON( key, ToHashtable() );   
		}
	}
}

