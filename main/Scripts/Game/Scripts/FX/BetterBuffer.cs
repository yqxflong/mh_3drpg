using System.Collections;
using System.Collections.Generic;

// By Joe Li
public class BetterBuffer<T>
{
	T[] m_Buffer = null;
	
	public bool Adjust(int size_probe, bool restore = false)
	{
		bool isBufferChanged = false;

		if(m_Buffer == null || size_probe > m_Buffer.Length)
		{
			T[] buffer = new T[CalculateBufferLength(size_probe)];
			if(restore && m_Buffer != null)
			{
				m_Buffer.CopyTo(buffer, 0);
			}
			m_Buffer = buffer;
			isBufferChanged = true;
		}

		return isBufferChanged;
	}

	public int Length
	{
		get
		{
			return (m_Buffer == null) ? 0 : m_Buffer.Length;
		}
	}

	public T[] GetBuffer()
	{
		return m_Buffer;
	}

	public T this[int i]
	{
		get
		{
			return m_Buffer[i];
		}

		set
		{
			m_Buffer[i] = value;
		}
	}

	int CalculateBufferLength(int size_probe)
	{
		int length = 4;

		while(length < size_probe)
		{
			length = length << 1;
		}

		return length;
	}
}
