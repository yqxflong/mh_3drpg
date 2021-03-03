using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class RedeemerItem : EB.Dot.IDotListItem
	{
		public RedeemerItem( Hashtable data = null )
		{
			if( data != null )
			{
				this.Type = EB.Dot.String("type", data, "");
				if( string.IsNullOrEmpty(this.Type))
				{
					this.Type = EB.Dot.String("t", data, "");
				}
				this.Data = EB.Dot.String("data", data, "");
				if( string.IsNullOrEmpty(this.Data))
				{
					this.Data = EB.Dot.String("n", data, "");
				}
				this.Quantity = EB.Dot.Integer("quantity", data, 0);
				if( this.Quantity == 0 )
				{
					this.Quantity = EB.Dot.Integer("q", data, 0);
				}
                this .Balance = EB.Dot.Integer("balance", data, 0);
                //if (this.Balance == 0){this.Balance = EB.Dot.Integer("b", data, 0);}
            }
			else
			{
				this.Type = string.Empty;
				this.Data = string.Empty;
				this.Quantity = 0;
			}
		}
		
		public RedeemerItem( string type, string data, int quantity )
		{
			this.Type = type;
			this.Data = data;
			this.Quantity = quantity;
		}
		
		public override string ToString()
		{
			return string.Format("Type:{0} Data:{1} Quantity:{2}", this.Type, this.Data, this.Quantity );
		}
		
		public bool IsValid
		{
			get
			{
				return (this.Quantity > 0) && (string.IsNullOrEmpty(this.Type) == false);
			}
		}
		
		public bool IsSameItem( RedeemerItem candidate )
		{
			return ( this.Type == candidate.Type ) && ( this.Data == candidate.Data );
		}
		
		public static bool operator ==( RedeemerItem lhs, RedeemerItem rhs )
		{
			//check if one side is null first
			if((object)rhs == null || (object)lhs == null)
			{
				if((object)rhs == null && (object)lhs == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			return ( lhs.Type == rhs.Type ) && ( lhs.Data == rhs.Data ) && ( lhs.Quantity == rhs.Quantity );
		}
		
		public static bool operator !=( RedeemerItem lhs, RedeemerItem rhs )
		{
			//check if one side is null first
			if((object)rhs == null || (object)lhs == null)
			{
				if((object)rhs == null && (object)lhs == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

			return ( lhs.Type != rhs.Type ) || ( lhs.Data != rhs.Data ) || ( lhs.Quantity != rhs.Quantity );
		}
		
		public override bool Equals(object obj)
		{
			RedeemerItem lhs = obj as RedeemerItem;
			if( lhs != null )
			{
				return lhs == this;
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ this.Data.GetHashCode() ^ this.Quantity.GetHashCode();
		} 

		public string Type { get; private set; }
		public string Data { get; private set; }
		public int Quantity { get; private set; }
        public int Balance { get; private set; }
	}
}

