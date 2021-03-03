namespace EB.Collections
{
	/// <summary>
	/// slow but non alloc queue
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Queue<T> : System.Collections.Generic.List<T>
	{
	    public Queue()
	    {
	
	    }
	
	    public Queue(int capacity) : base(capacity)
	    {
	
	    }
	
	    public void Enqueue(T obj)
	    {
	        Add(obj);
	    }
	
	    public T Peek()
	    {
	        return this[0];
	    }
	
	    public T Dequeue()
	    {
	        T value = this[0];
	        RemoveAt(0);
	        return value;
	    }
	}

	/// <summary>
	/// fast but alloc queue
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FastQueue<T> : System.Collections.Generic.LinkedList<T>
	{
		public FastQueue()
		{

		}

		public void Enqueue(T obj)
		{
			AddLast(obj);
		}

		public T Peek()
		{
			return First.Value;
		}

		public T Dequeue()
		{
			T value = First.Value;
			RemoveFirst();
			return value;
		}

		public void Insert(int index, T value)
		{
			int count = Count;

			if (index > count || index < 0)
			{
				throw new System.ArgumentOutOfRangeException("index", index, "invalid index");
			}

			if (index == count)
			{
				AddLast(value);
				return;
			}

			int i = 0;
			System.Collections.Generic.LinkedListNode<T> node = First;
			while (node != null && i++ < index)
			{
				node = node.Next;
			}
			AddBefore(node, value);
		}

		public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection)
		{
			int count = Count;

			if (index > count || index < 0)
			{
				throw new System.ArgumentOutOfRangeException("index", index, "invalid index");
			}

			if (index == count)
			{
				var tailIter = collection.GetEnumerator();
				while (tailIter.MoveNext())
				{
					AddLast(tailIter.Current);
				}
				tailIter.Dispose();
				return;
			}

			int i = 0;
			System.Collections.Generic.LinkedListNode<T> node = First;
			while (node != null && i++ < index)
			{
				node = node.Next;
			}

			var iter = collection.GetEnumerator();
			while (iter.MoveNext())
			{
				AddBefore(node, iter.Current);
			}
			iter.Dispose();
		}

		public T[] ToArray()
		{
			T[] array = new T[Count];
			CopyTo(array, 0);
			return array;
		}
	}

	public class Stack<T> : System.Collections.Generic.List<T>
	{
	    public Stack() { }

		public Stack(int capacity) : base(capacity) { }
	
	    public void Push(T obj)
	    {
	        Add(obj);
	    }
	
	    public T Peek()
	    {
	        return this[Count - 1];
	    }
	
	    public T Pop()
	    {
	        T value = this[Count - 1];
	        RemoveAt(Count - 1);
	        return value;
	    }

		// top to bottom order
		public new T[] ToArray()
		{
			T[] result = base.ToArray();
			System.Array.Reverse(result);
			return result;
		}
	}

	// Basic Tuple since mono doesn't support .net tuples.
	public class Tuple<T1, T2>
	{
		public T1 Item1;
		public T2 Item2;

		public Tuple(T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}
	}

	public class CircularBuffer : System.Collections.IEnumerable
	{
		private object[] _buffer;
		private int _head = 0;
		private int _tail = 0;
		
		public CircularBuffer( int size )
		{
			_buffer = new object[size];
		}
		
		public void Clear()
		{
			_head = _tail = 0;
		}
		
		public int Count
		{
			get
			{
				return ((_tail+_buffer.Length)-_head) % _buffer.Length;
			}
		}
		
		public object[] ToArray()
		{
			var array = new object[Count];
			for ( int i = 0; i < array.Length; ++i )
			{
				int idx = (_head+i)%_buffer.Length;
				array[i] = _buffer[idx];
			}
			return array;
		}
		
		public void Push( object obj )
		{
			_buffer[_tail] = obj;
			_tail = (_tail+1)%_buffer.Length;
			if ( _tail == _head )
			{
				_head = (_head+1)%_buffer.Length;;
			}
		}
		
		public System.Collections.IEnumerator GetEnumerator ()
		{
			int i = _head;
			while ( i != _tail )
			{
				yield return _buffer[i];
				i = (i+1)%_buffer.Length;
			}
		}
	}
}