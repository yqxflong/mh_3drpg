using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class RewardStageData
	{
		public int Id;
		public int Stage;
		public int ActivityId;
		public List<LTShowItemData> Awards;
		public eReceiveState ReceiveState;

		public RewardStageData(int id, int stage, LTShowItemData award, eReceiveState state = eReceiveState.cannot)
		{
			this.Id = id;
			this.Stage = stage;
			this.Awards = new List<LTShowItemData>();
			this.Awards.Add(award);
			this.ReceiveState = state;
		}

		public RewardStageData(int id, int stage, List<LTShowItemData> awards, eReceiveState state)
		{
			this.Id = id;
			this.Stage = stage;
			this.Awards = awards;
			this.ReceiveState = state;
		}
		public RewardStageData(int id, int activityId, int stage, List<LTShowItemData> awards, eReceiveState state)
		{
			this.Id = id;
			this.ActivityId = activityId;
			this.Stage = stage;
			this.Awards = awards;
			this.ReceiveState = state;
		}
	}
}