///////////////////////////////////////////////////////////////////////
//
//  OrderedHashtable.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections;

public class OrderedHashtable : Hashtable
{
	public class OrderedHashtableEnumerator : IDictionaryEnumerator
	{
		// A copy of the SimpleDictionary object's key/value pairs.
		DictionaryEntry[] items;
		System.Int32 index = -1;

		public OrderedHashtableEnumerator(OrderedHashtable table)
		{
			// Make a copy of the dictionary entries currently in the SimpleDictionary object.
			items = new DictionaryEntry[table.Count];
			int i = 0;
			foreach (object key in table.Keys)
			{
				items[i] = new DictionaryEntry(key, table[key]);
				i++;	
			}

			System.Array.Sort(items, delegate(DictionaryEntry a, DictionaryEntry b) {
				int ret = 0;
				try
				{
					ret = (a.Key as System.IComparable).CompareTo(b.Key as System.IComparable);
				}
				catch (System.Exception e)
				{
					EB.Debug.LogWarning(e);
				}
				return ret;
			});
		}

		// Return the current item. 
		public System.Object Current { get { ValidateIndex(); return items[index]; } }

		// Return the current dictionary entry. 
		public DictionaryEntry Entry
		{
			get { return (DictionaryEntry) Current; }
		}

		// Return the key of the current item. 
		public System.Object Key { get { ValidateIndex();  return items[index].Key; } }

		// Return the value of the current item. 
		public System.Object Value { get { ValidateIndex();  return items[index].Value; } }

		// Advance to the next item. 
		public bool MoveNext()
		{
			if (index < items.Length - 1) { index++; return true; }
			return false;
		}

		// Validate the enumeration index and throw an exception if the index is out of range. 
		private void ValidateIndex()
		{
			if (index < 0 || index >= items.Length)
			throw new System.InvalidOperationException("Enumerator is before or after the collection.");
		}

		// Reset the index to restart the enumeration. 
		public void Reset()
		{
			index = -1;
		}
	}

	public override IDictionaryEnumerator GetEnumerator()
	{
		return new OrderedHashtableEnumerator(this);
	}
}