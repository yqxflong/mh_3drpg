//this data will only allocate in fixed size chunks
public class FixedAllocationList<T> 
{
	/*
	int allocationSize;
	private Queue<T> data;

	public FixedAllocationList(int allocSize)
	{
		data = new Queue<T>(allocSize);
		allocationSize = allocSize;
	}

	public T Get(int index)
	{
		return data[index];
	}

	public void PushFront(T value)
	{
		data.Enqueue(value);
	}

	public void Insert(int index, T value)
	{
		while (data.Count >= data.Capacity || index >= data.Capacity)
		{
			data.Capacity += allocationSize;
		}
		data.Insert(index, value);
	}

	public void Remove(int index)
	{
		data.RemoveAt(index);
	}

	public int Count()
	{
		return data.Count;
	}
	*/
}
