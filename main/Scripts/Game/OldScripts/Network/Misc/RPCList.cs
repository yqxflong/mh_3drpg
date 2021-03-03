///////////////////////////////////////////////////////////////////////
//
//  RPCList.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Collections;
using EB.Replication;

//Warning you must register each type of list you intend to use in Setup.cs
public class RPCList<T> : ISerializable, IEnumerable where T : ISerializable, new()
{
	private List<T> _list;
	public List<T> UnderlyingList
	{
		get
		{
			return _list;
		}
	}

	public int Count
	{
		get
		{
			return _list.Count;
		}
	}

	public T this[int index]
	{
		get
		{
			return _list[index];
		}
	}

	public RPCList()
	{
		_list = new List<T>();
	}

	public RPCList(List<T> copy)
	{
		_list = copy;
	}

	public void Add(T element)
	{
		_list.Add(element);
	}

	public IEnumerator GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	public void Serialize(EB.BitStream bs)
	{
		int numElements = 0;
		if (!bs.isReading)
		{
			numElements = _list.Count;
		}
		bs.Serialize(ref numElements);

		if (bs.isReading)
		{
			for (int i = 0; i < numElements; i++)
			{
				T element = new T();
				element.Serialize(bs);
				_list.Add(element);
			}
		}
		else
		{
			for (int i = 0; i < numElements; i++)
			{
				_list[i].Serialize(bs);
			}
		}
	}
}