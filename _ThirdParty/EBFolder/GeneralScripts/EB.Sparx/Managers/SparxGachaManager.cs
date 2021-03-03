using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class GachaConfig
	{
		public string[] Groups = new string[] { "base" };
	}
	
	public class GachaPickItem
	{
		public GachaPickItem( Hashtable data = null )
		{
			if( data != null )
			{
				this.RewardType = EB.Dot.String("type", data, "");
				this.RewardData = EB.Dot.String("data", data, "");
				this.RewardCount = EB.Dot.Integer("quantity", data, 0);
				if( this.RewardCount == 0 )
				{
					this.RewardCount = EB.Dot.Integer("count", data, 0);
				}
			}
			else
			{
				this.RewardType = string.Empty;
				this.RewardData = string.Empty;
				this.RewardCount = 0;
			}
		}
		
		public override string ToString()
		{
			return string.Format("RewardType:{0} RewardData:{1} RewardCount:{2}", this.RewardType, this.RewardData, this.RewardCount );
		}
		
		public string RewardType { get; private set; }
		public string RewardData { get; private set; }
		public int RewardCount { get; private set; }
	}
	
	public class GachaPickResult
	{
		public GachaPickResult( Hashtable data = null )
		{
			if( data != null )
			{
				this.SoftCurrentToPay = EB.Dot.Integer("softToPay", data, 0);
				this.XpToGive = EB.Dot.Integer("xpToGive", data, 0);
				ArrayList items = EB.Dot.Array("items", data, null );
				if( items != null )
				{
					List<GachaPickItem> pickItems = new List<GachaPickItem>();
					foreach( Hashtable item in items )
					{
						GachaPickItem pickItem = new GachaPickItem( item );
						pickItems.Add( pickItem );
					}
					this.Items = pickItems.ToArray();
				}
				else
				{
					this.Items = new GachaPickItem[0];
				}
			}
			else
			{
				
				this.SoftCurrentToPay = 0;
				this.XpToGive = 0;
				this.Items = new GachaPickItem[0];
			}
		}
		
		public override string ToString()
		{
			return string.Format("SoftCurrentToPay:{0} XpToGive:{1} Items #:{2}", this.SoftCurrentToPay, this.XpToGive, this.Items.Length );
		}
		
		public int SoftCurrentToPay { get; private set; }
		public int XpToGive { get; private set; }
		public GachaPickItem[] Items { get; private set; }
	}
	
	public class GachaPossiblePrize
	{
		public GachaPossiblePrize( Hashtable data = null )
		{
			this.Type = string.Empty;
			this.Data = string.Empty;
		
			if( data != null )
			{
				this.Type = EB.Dot.String( "type", data, string.Empty );
				this.Data = EB.Dot.String( "data", data, string.Empty );
			}
		}
		
		public bool IsValid
		{
			get
			{
				return string.IsNullOrEmpty( this.Type ) == false;
			}
		}
	
		public string Type { get; private set; }
		public string Data { get; private set; }
	}
	
	public class GachaBoxSpendInfo
	{
		public GachaBoxSpendInfo( Hashtable data = null )
		{
			this.Cost = 0;
			this.Xp = 0;
		
			if( data != null )
			{
				this.Cost = EB.Dot.Integer( "cost", data, 0 );
				this.Xp = EB.Dot.Integer( "xp", data, 0 );
			}
		}
		
		public override string ToString()
		{
			return string.Format("Cost:{0} Xp:{1}", this.Cost, this.Xp );
		}
	
		public int Cost { get; private set; }
		public int Xp { get; private set; }
	}
	
	public class GachaBox
	{
		public GachaBox( int index, Hashtable data = null )
		{
			this.Name = string.Empty;
			this.Index = index;
			this.Token = string.Empty;
			this.Image = string.Empty;
			this.Multiplier = 0;
			this.PossiblePrizes = new List<GachaPossiblePrize>();

			
			if( data != null )
			{
				this.Name = EB.Dot.String( "name", data, string.Empty );
				this.Token = EB.Dot.String( "token", data, string.Empty );
				this.Image = EB.Dot.String( "image", data, string.Empty );
				this.Multiplier = EB.Dot.Integer( "multiplier", data, 0 );
				ArrayList possiblePrizesList = EB.Dot.Array( "possiblePrizes", data, null );
				if( possiblePrizesList != null )
				{
					foreach( object candidate in possiblePrizesList )
					{
						GachaPossiblePrize possiblePrize = new GachaPossiblePrize( candidate as Hashtable );
						if( possiblePrize.IsValid == true )
						{
							this.PossiblePrizes.Add( possiblePrize );
						}
					}
				}
				this.SoftCurrency = new GachaBoxSpendInfo( EB.Dot.Object( "sc", data, null ) );
				this.HardCurrency = new GachaBoxSpendInfo( EB.Dot.Object( "hc", data, null ) );
				this.Tokens = new GachaBoxSpendInfo( EB.Dot.Object( "tokenc", data, null ) );
			}
			else
			{
				this.SoftCurrency = new GachaBoxSpendInfo();
				this.HardCurrency = new GachaBoxSpendInfo();
				this.Tokens = new GachaBoxSpendInfo();
			}
		}
		
		public bool IsValid
		{
			get
			{
				return string.IsNullOrEmpty( this.Name ) == false;
			}
		}
		
		public override string ToString()
		{
			var possiblePrizes = new System.Text.StringBuilder();
			foreach( GachaPossiblePrize prize in this.PossiblePrizes )
			{
				possiblePrizes.Append( prize.Type );
				possiblePrizes.Append( "->" );
				possiblePrizes.Append( prize.Data );
				possiblePrizes.Append( "," );
			}
			return string.Format("Name:{0} Index:{1} Multiplier:{2} SC:{3} HC:{4} Token:{5} PossiblePrizes:{6}", this.Name, this.Index, this.Multiplier, this.SoftCurrency, this.HardCurrency, this.Tokens, possiblePrizes );
		}
	
		public string Name { get; private set; }
		public int Index { get; private set; }
		public string Token { get; private set; }
		public string Image { get; private set; }
		public int Multiplier { get; private set; }
		public GachaBoxSpendInfo SoftCurrency { get; private set; }
		public GachaBoxSpendInfo HardCurrency { get; private set; }
		public GachaBoxSpendInfo Tokens { get; private set; }
		public List<GachaPossiblePrize> PossiblePrizes { get; private set; }
	}
	
	public class GachaSet
	{
		public GachaSet( Hashtable data = null )
		{
			this.Version = string.Empty;
			this.Name = string.Empty;
			this.AttractImages = new List<string>();
			this.PossiblePrizes = new List<GachaPossiblePrize>();
			this.Boxes = new List<GachaBox>();
			
			if( data != null )
			{
				this.Version = EB.Dot.String( "version", data, string.Empty );
				this.Name = EB.Dot.String( "name", data, string.Empty );
				ArrayList attractsList = EB.Dot.Array( "attracts", data, null );
				if( attractsList != null )
				{
					foreach( object candidate in attractsList )
					{
						if( candidate is string )
						{
							this.AttractImages.Add( candidate as string );
						}
					}
					this.AttractImages.Reverse();
				}
				ArrayList possiblePrizesList = EB.Dot.Array( "possiblePrizes", data, null );
				if( possiblePrizesList != null )
				{
					foreach( object candidate in possiblePrizesList )
					{
						GachaPossiblePrize possiblePrize = new GachaPossiblePrize( candidate as Hashtable );
						if( possiblePrize.IsValid == true )
						{
							this.PossiblePrizes.Add( possiblePrize );
						}
					}
					this.PossiblePrizes.Reverse();
				}
				ArrayList boxesList = EB.Dot.Array( "boxes", data, null );
				if( boxesList != null )
				{
					for( int i = 0; i < boxesList.Count; ++i )
					{
						GachaBox box = new GachaBox( i, boxesList[ i ] as Hashtable );
						if( box.IsValid == true )
						{
							this.Boxes.Add( box );
						}
					}
				}
			}
		}
		
		public bool IsValid
		{
			get
			{
				return ( string.IsNullOrEmpty( this.Name ) == false ) && ( string.IsNullOrEmpty( this.Version ) == false );
			}
		}
		
		public override string ToString()
		{
			var possiblePrizes = new System.Text.StringBuilder();
			foreach( GachaPossiblePrize prize in this.PossiblePrizes )
			{
				possiblePrizes.Append( prize.Type );
				possiblePrizes.Append( "->" );
				possiblePrizes.Append( prize.Data );
				possiblePrizes.Append( "," );
			}
		
			return string.Format("Name:{0} Boxes:{3} Attracts:{1} PossiblePrizes:{2}", this.Name, string.Join( ",", this.AttractImages.ToArray() ), possiblePrizes, Boxes.Count );
		}
	
		public string Version { get; private set; }
		public string Name { get; private set; }
		public List<string> AttractImages { get; private set; }
		public List<GachaPossiblePrize> PossiblePrizes { get; private set; }
		public List<GachaBox> Boxes { get; private set; }
	}

	public class GachaManager : SubSystem, Updatable
	{
		GachaConfig	_config = new GachaConfig();
		GachaAPI _api = null;
		Dictionary< string, GachaSet > _sets = new Dictionary< string, GachaSet >();
		bool _initialTokensComplete = false;
		bool _setsComplete = false;
		Dictionary< string, int > _tokens = new Dictionary<string, int>();
		
		public int GetTokenCount( string box )
		{
			int tokens = 0;
			if( this._tokens.TryGetValue( box, out tokens ) == false )
			{
				tokens = 0;
			}
			return tokens;
		}
		
		public GachaSet GetGachaSet( string name )
		{
			GachaSet gachaSet = null;
			if( this._sets.TryGetValue( name, out gachaSet ) == false )
			{
				gachaSet = null; 
			}
			
			return gachaSet;
		}
		
		public void PickFromBox( string group, string setName, string box, string payment, System.Action<string,GachaPickResult> onComplete)
		{
			GachaSet set = null;
			if( this._sets.TryGetValue( group, out set ) == true )
			{
				if( set.Name != setName )
				{
					set = null;
				}
			}
			else
			{
				set = null;
			}

			if( set != null )
			{
				this._api.PickFromBox( group, set.Version, set.Name, box, payment, delegate( string error, Hashtable data ){
					if( data != null )
					{
						GachaPickResult pickResult = new GachaPickResult( data );
						if( pickResult.Items.Length > 0 )
						{
							if( payment == "token" )
							{
								int balance = EB.Dot.Integer( "balance", data, -1 );
								if( balance != -1 )
								{
									this._tokens[ box ] = balance;
								}
							}
							onComplete( null, pickResult );
						}
						else
						{
							string errMsg = ( error != null ) ? error : string.Format( "Couldn't afford the Gacha Spin {0}-{1}", box, payment );
							EB.Debug.LogError( errMsg );
							onComplete( errMsg, null );
						}
					}
					else
					{
						EB.Debug.LogError("Server responded with error to Gacha Pick: '{0}'", error );
						onComplete( error, null );
					}
				});
			}
			else
			{
				string error = string.Format( "Requested a pick on an unknown set: {0}", setName );
				EB.Debug.LogError( error );
				onComplete( error, null );
			}
		}
		
		public void Sync( System.Action< Dictionary<string, int> > onComplete )
		{
			_api.SyncTokens( delegate( string error, ArrayList data ) {
				this.OnSyncTokens( error, data );
				if( onComplete != null ) {
					onComplete( this._tokens );
				}
			});
		}
		
		#region implemented abstract members of EB.Sparx.Manager
		public override void Initialize (Config config)
		{
			this._config = config.GachaConfig;
			_api = new GachaAPI(Hub.ApiEndPoint);
		}
		
		public bool UpdateOffline { get { return false;} }
		
		public override void Connect()
		{
			var gachaData = Dot.Object( "gacha", Hub.DataStore.LoginDataStore.LoginData, null );
			if( gachaData != null )
			{
				var sets = Dot.Object( "sets", gachaData, null );
				if( sets != null ){
					this.OnFetchSets( null, sets );
				}
				else
				{
					this._api.FetchSets( this._config.Groups, OnFetchSets);
				}
				
				var tokens = Dot.Array( "tokens", gachaData, null );
				if( tokens != null )
				{
					this.OnSyncTokens( null, tokens);
				}
				else 
				{
					this.Sync( null );
				}
			}
			else
			{
				this.Sync( delegate( Dictionary< string, int > tokens ){} );
				this._api.FetchSets( this._config.Groups, OnFetchSets);
			}
		}

		public void Update ()
		{
		}

		public override void Disconnect (bool isLogout)
		{
		}
		#endregion
		
		public override void Async (string message, object payload)
		{
			switch(message.ToLower())
			{
				case "sync":
				{
					Sync( null );
					break;
				}
				default:
				{
					break;
				}
			}
		}

		private void OnSyncTokens( string error, ArrayList data )
		{
			if( data != null )
			{
				for(int i = 0; i < data.Count; i++)
				{
					string name = EB.Dot.String( "token", data[i], string.Empty );
					int count = EB.Dot.Integer( "count", data[i], 0 );
					if( name.Length > 0 )
					{
						this._tokens[ name ] = count;
					}
				}
				
				this._initialTokensComplete = true;
				if( State == SubSystemState.Connecting )
				{
					if( ( this._initialTokensComplete == true ) && ( this._setsComplete == true ) )
					{
						State = SubSystemState.Connected;
					}
				}
			}
			else
			{
				EB.Debug.LogError( "Problem getting tokens in Sync. Error:{0}", error );
			}
		}
		
		private void OnFetchSets( string error, Hashtable data )
		{
			if( string.IsNullOrEmpty( error ) == true )
			{
				if( data != null )
				{
					foreach( string group in this._config.Groups )
					{
						Hashtable groupData = data[ group ] as Hashtable;
						if( groupData != null )
						{
							GachaSet gachaSet = new GachaSet( groupData );
							if( gachaSet.IsValid == true )
							{
								this._sets[ group ] = gachaSet;
							}
							else
							{
								EB.Debug.LogError( "GachaManager.OnSetFetch Group {0} was inproperly formatted from the server", group );
								State = SubSystemState.Error;
							}
						}
					}
				}
				this._setsComplete = true;
			}
			else
			{
				EB.Debug.LogError( "GachaManager.OnFetchSets got Error:{0}", error );
				State = SubSystemState.Error;
			}
			
			if( State == SubSystemState.Connecting )
			{
				if( ( this._initialTokensComplete == true ) && ( this._setsComplete == true ) )
				{
					State = SubSystemState.Connected;
				}
			}
		}
	}
}
