using UnityEngine;

namespace MoveEditor
{
	public class PropPreview
	{
		public void Init(PropsController propsController, string propName, bool enable, float startTime)
		{
			_propsController	= propsController;
			_propName 			= propName;
			_enable 			= enable;
			_startTime 			= startTime;

			Reset();
		}

		public void Reset()
		{
			_played = false;
		}

		public void Update(float time)
		{
			if (!_played)
			{
				float t = time - _startTime;

				if (t >= 0)
				{
					_propsController.ToggleProp(_propName, _enable);
					_played = true;
				}
			}
		}
		private PropsController	_propsController	= null;
		private string			_propName			= "";
		private bool			_enable				= false;
		private float			_startTime			= 0;
		private bool			_played				= false;
	}
}