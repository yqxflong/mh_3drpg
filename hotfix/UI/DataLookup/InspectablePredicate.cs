using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	[System.Serializable]
	public class InspectablePredicate
	{
		public enum LogicalOperatorValue { AND, OR };

		public enum ComparaisonOperatorValue
		{
			EQUAL_TO, NOT_EQUAL_TO,
			GREATER_THAN, GREATER_THAN_OR_EQUAL_TO,
			LESS_THAN, LESS_THAN_OR_EQUAL_TO
		};

		//使用struct 的Part会导致Query中的Part不能修改 因为struct是值传递的，这个不利于做动态筛选
		[System.Serializable]
		public /*struct*/class Part
		{
			public LogicalOperatorValue PreviousPartLogicOperator;
			public string PropertyName;
			public ComparaisonOperatorValue ComparaisonOperator;
			public string Value;
		}

		public List<Part> Query;

		public System.Predicate<IDictionary> ToNativePredicate()
		{
			return delegate (IDictionary obj)
			{
				bool result = true;

				if (Query != null)
				{
					for (var i = 0; i < Query.Count; i++)
					{
						Part part = Query[i];
						bool isAndPreviousPart = part.PreviousPartLogicOperator == LogicalOperatorValue.AND;

						if (isAndPreviousPart && !result)
							continue;

						bool partResult = true;

						if (!obj.Contains(part.PropertyName))
						{
							partResult = false;
							EB.Debug.LogWarning("[InspectablePredicate]ToNativePredicate: Property '{0}' not found on tested object!" , part.PropertyName);
							break;
							//throw new InvalidOperationException(string.Format("Property '{0}' not found on tested object", part.PropertyName));
						}

						string valueA = obj[part.PropertyName].ToString();

						string valueB = part.Value.ToString();

						switch (part.ComparaisonOperator)
						{
							case ComparaisonOperatorValue.EQUAL_TO: partResult = (valueA.Equals(valueB)); break;
							case ComparaisonOperatorValue.NOT_EQUAL_TO: partResult = !(valueA.Equals(valueB)); break;
								// TODO : implements missing operators there
						}

						result = isAndPreviousPart ? result && partResult : result || partResult;
					}
				}

				return result;
			};
		}
	}
}