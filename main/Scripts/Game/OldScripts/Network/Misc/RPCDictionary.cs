///////////////////////////////////////////////////////////////////////
//
//  RPCDictionary.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class RPCDictionary : EB.Replication.ISerializable
{
	private Dictionary<string, object> _dictionary;

	public Dictionary<string, object> UnderlyingDictionary
	{
		get
		{
			return _dictionary;
		}
	}

	public object this[string key]
	{
		get
		{
			return _dictionary[key];
		}
		set
		{
			_dictionary[key] = value;
		}
	}

	public RPCDictionary()
	{
		_dictionary = new Dictionary<string, object>();
	}

	public RPCDictionary(Dictionary<string, object> copy)
	{
		_dictionary = copy;
	}

	public void Serialize(EB.BitStream bs)
	{
		int numKeys = 0;
		if (!bs.isReading)
		{
			numKeys = _dictionary.Count;
		}
		bs.Serialize(ref numKeys);

		object[] keysAndValues = new object[numKeys * 2];

		if (!bs.isReading)
		{
			int i = 0;
			foreach (string key in _dictionary.Keys)
			{
				keysAndValues[i] = key;
				keysAndValues[i + 1] = _dictionary[key];
				i += 2;
			}
		}

		EB.Replication.Manager.SerializeVarAgs(bs, ref keysAndValues);

		if (bs.isReading)
		{
			for (int i = 0; i < keysAndValues.Length; i += 2)
			{
				_dictionary.Add((string)keysAndValues[0], (object)keysAndValues[1]);
			}
		}
	}
}