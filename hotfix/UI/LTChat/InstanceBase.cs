namespace Hotfix_LT.UI
{
	public class InstanceBase<T> where T : class, new()
	{
		private static T instanceInternal = null;

		public static T instance
		{
			get
			{
				if (instanceInternal == null)
				{
					instanceInternal = new T();
				}

				return instanceInternal;
			}
		}
	}
}