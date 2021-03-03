using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipmentColorDataLookup : DataLookup 
{
	protected Dictionary<string, string> m_EquipmentRegisterIDs = new Dictionary<string, string>();

	public void RegisterEquipments(IDictionary equipments)
	{
		if(equipments == null)
		{
			return;
		}
		ClearRegisteredDataIDs();
		m_EquipmentRegisterIDs.Clear();

		IDictionaryEnumerator enumerator = equipments.GetEnumerator();
		while(enumerator.MoveNext())
		{
			if(enumerator.Key != null && enumerator.Value != null)
			{
				string dataID = string.Format("{0}.equipmentColorIndex", enumerator.Value.ToString());
				m_EquipmentRegisterIDs.Add (enumerator.Key.ToString(), dataID);
				RegisterDataID(dataID);
			}
		}
	}

	public override void OnLookupUpdate (string dataID, object value)
	{
		base.OnLookupUpdate (dataID, value);
		if(value == null)
		{
			return;
		}

		Dictionary<string, string>.Enumerator enumerator = m_EquipmentRegisterIDs.GetEnumerator();
		while(enumerator.MoveNext())
		{
			KeyValuePair<string, string> current = enumerator.Current;
			AvatarComponent avatar = GetComponent<AvatarComponent>();
			int colorIndex = 0;
			if(dataID == enumerator.Current.Value)
			{
				if(int.TryParse(value.ToString(), out colorIndex))
				{
					if(avatar != null)
					{
						avatar.UpdateEquipmentColor(current.Key, colorIndex - 1);
					}
				}
			}
		}
	}
}
