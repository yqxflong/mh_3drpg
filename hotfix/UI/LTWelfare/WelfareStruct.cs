namespace Hotfix_LT.UI
{
	public class SignInData : INodeData
	{
		public int Num;  //已经签到的天数
		public int HaveResigninNum;
		public bool IsSigned;

		public void CleanUp()
		{
			Num = 0;
			HaveResigninNum = 0;
			IsSigned = false;
		}

		public object Clone()
		{
			return new SignInData();
		}

		public void OnUpdate(object obj)
		{
			Num = EB.Dot.Integer("count", obj, 0);
			HaveResigninNum = EB.Dot.Integer("additional_count", obj, 0);
			IsSigned = EB.Dot.Bool("is_signed", obj, false);
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}
	}

	public class EverydayAwardInfo : INodeData
	{
		public bool IsMonthlyVip;
		public int ResidueMonthCardDay;
		public bool IsHaveReceiveMonthCardGift;

		public bool IsAllianceMonthlyVip;
		public int ResidueAllianceMonthCardDay;
		public bool IsHaveReceiveAllianceMonthCardGift;

		//private double MonthlyVipExpireTime;
		//private double AllianceMonthlyExpireTime;

		public bool IsHaveShare;
		public bool IsHaveReceiveShareGift;
		public bool IsHaveReceiveLoginGift;
		public bool IsHaveReceiveVIPGift;
		public bool IsHaveReceiveVogirGift1;
		public bool IsHaveReceiveVogirGift2;

		public bool IsHaveReceiveFirstChargeGift;

		public int FirstChargeLoginDay;
		public bool IsHaveReceiveFirstChargeGift1;
		public bool IsHaveReceiveFirstChargeGift2;
		public bool IsHaveReceiveFirstChargeGift3;

		public void CleanUp()
		{
			IsMonthlyVip = false;
			ResidueMonthCardDay = 0;
			IsHaveReceiveMonthCardGift = false;
			IsAllianceMonthlyVip = false;
			ResidueAllianceMonthCardDay = 0;
			IsHaveReceiveAllianceMonthCardGift = false;

			IsHaveShare = false;
			IsHaveReceiveShareGift = false;
			IsHaveReceiveLoginGift = false;
			IsHaveReceiveVIPGift = false;
			IsHaveReceiveVogirGift1 = false;
			IsHaveReceiveVogirGift2 = false;

			IsHaveReceiveFirstChargeGift = false;
			IsHaveReceiveFirstChargeGift1 = false;
			IsHaveReceiveFirstChargeGift2 = false;
			IsHaveReceiveFirstChargeGift3 = false;
		}

		public object Clone()
		{
			return new EverydayAwardInfo();
		}

		public void OnUpdate(object obj)
		{
			IsMonthlyVip = EB.Dot.Bool("monthly_vip.is_monthly_vip", obj, IsMonthlyVip);
			//double charge_time = Dot.Double("monthly_vip.charge_time", obj, 0f);
			double expire_time = EB.Dot.Double("monthly_vip.expire_time", obj, 0);
			if ((int)expire_time > 0)
			{
				int day = System.TimeSpan.FromSeconds(expire_time - EB.Time.Now).Days;
				ResidueMonthCardDay = day > 0 ? day : 0;
			}

			IsHaveReceiveMonthCardGift = EB.Dot.Bool("monthly_vip.is_draw", obj, IsHaveReceiveMonthCardGift);

			IsAllianceMonthlyVip = EB.Dot.Bool("alli_monthly_vip.is_monthly_vip", obj, IsAllianceMonthlyVip);
			//double charge_time = EB.Dot.Double("monthly_vip.charge_time", obj, 0f);
			expire_time = EB.Dot.Double("alli_monthly_vip.expire_time", obj, 0);
			if ((int)expire_time > 0)
			{
				int day = System.TimeSpan.FromSeconds(expire_time - EB.Time.Now).Days;
				ResidueAllianceMonthCardDay = day > 0 ? day : 0;
			}
			IsHaveReceiveAllianceMonthCardGift = EB.Dot.Bool("alli_monthly_vip.is_draw", obj, IsHaveReceiveAllianceMonthCardGift);

			IsHaveShare = EB.Dot.Bool("daily_share.is_shared", obj, IsHaveShare);
			IsHaveReceiveShareGift = EB.Dot.Bool("daily_share.is_draw_share_reward", obj, IsHaveReceiveShareGift);
			IsHaveReceiveLoginGift = EB.Dot.Bool("daily_login.is_draw_daily_reward", obj, IsHaveReceiveLoginGift);
			IsHaveReceiveVIPGift = EB.Dot.Bool("daily_login.is_draw_daily_vip", obj, IsHaveReceiveVIPGift);
			IsHaveReceiveVogirGift1 = EB.Dot.Bool("daily_login.is_draw_daily_vigor1", obj, IsHaveReceiveVogirGift1);
			IsHaveReceiveVogirGift2 = EB.Dot.Bool("daily_login.is_draw_daily_vigor2", obj, IsHaveReceiveVogirGift2);

			IsHaveReceiveFirstChargeGift = EB.Dot.Bool("charge_reward.is_draw_first_charge", obj, IsHaveReceiveFirstChargeGift);

			FirstChargeLoginDay = EB.Dot.Integer("charge_reward.login_days", obj, FirstChargeLoginDay);
			IsHaveReceiveFirstChargeGift1 = EB.Dot.Bool("charge_reward.is_draw_first_charge_1", obj, IsHaveReceiveFirstChargeGift1);
			IsHaveReceiveFirstChargeGift2 = EB.Dot.Bool("charge_reward.is_draw_first_charge_2", obj, IsHaveReceiveFirstChargeGift2);
			IsHaveReceiveFirstChargeGift3 = EB.Dot.Bool("charge_reward.is_draw_first_charge_3", obj, IsHaveReceiveFirstChargeGift3);
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}
	}
}
