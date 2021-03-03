public class FrameAllocator<T> 
{
	public class AllocationData<T2>
	{
		private System.ArraySegment<T2> data;

		public AllocationData(System.ArraySegment<T2> d)
		{
			data = d;
		}
		
		public void Set(int index, T2 value)
		{
			data.Array[data.Offset + index] = value;
		}
		
		public T2 Get(int index)
		{
			return data.Array[data.Offset + index];
		}

		public T2[] GetArray()
		{
			return data.Array;
		}

		public int GetAllocationOffset()
		{
			return data.Offset;
		}

		public System.ArraySegment<T2> SliceArraySegment()
		{
			EB.Debug.LogWarning("OFFSET {0} COUNT {1}", data.Offset, data.Count);
			return new System.ArraySegment<T2>( data.Array, data.Offset,  data.Count); 
		}

		
	}
	
	//public AllocationData<T> AllocationData { get { return AllocationData<T>; } }

	public struct Allocation
	{
		public int size;
		public AllocationData<T> values;
	}
	
	int index;
	private T[] values;

	int allocationIndex;
	Allocation[] allocations;

	public FrameAllocator(int size, int maxAllocationsPerFrame = 20)
	{
		index = 0;
		allocationIndex = 0;
		
		values = new T[size];
		allocations = new Allocation[maxAllocationsPerFrame];
	}

	public void Update()
	{
		index = 0;
		allocationIndex = 0;
	}

	public Allocation Allocate(int size)
	{
		if (index + size >= values.Length)
		{
			EB.Debug.LogWarning("Out of space for an allocation of " + size + " in FrameAllocator, only " + (values.Length - index) + " left");
			size = values.Length - index;
		}

		Allocation allocation;
		if (allocationIndex >= allocations.Length)
		{
			EB.Debug.LogWarning("Out of allocations in VertexAllocator; increase maxAllocationsPerFrame to avoid memory allocations");
			allocation = new Allocation();
		}
		else
		{
			allocation = allocations[allocationIndex]; 
		}

		allocation.size = size;
		allocation.values = new AllocationData<T>(new System.ArraySegment<T>(values, index, size));

		index++;
		allocationIndex++;

		return allocation;
	}
}
