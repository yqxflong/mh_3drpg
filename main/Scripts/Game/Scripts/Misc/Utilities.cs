public static class Utilities
{
	public static bool AreFloatsEqual(float A, float B)
	{
		return (!(UnityEngine.Mathf.Abs(A - B) > UnityEngine.Mathf.Epsilon));
	}
	
	public static System.Attribute GetAttributeOfType(object[] attributes, System.Type type)
	{
		foreach (System.Attribute attr in attributes)
		{
			if (attr.GetType() == type)
			{
				return attr;
			}
		}

		return null;
	}

	public static object GetFirstElement(object[] array)
	{
		if (array == null)
		{
			return null;
		}
		else
		{
			return array[0];
		}
	}

	public static UnityEngine.Color HexToColor(string hex)
	{
		hex = hex.ToLower();
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new UnityEngine.Color32(r,g,b, 255);
	}

	public static T[] Shuffle<T>(T[] array)
	{
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
			int j = UnityEngine.Random.Range(0, i); // 0 <= j <= i-1
			// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
		return array;
	}
}
