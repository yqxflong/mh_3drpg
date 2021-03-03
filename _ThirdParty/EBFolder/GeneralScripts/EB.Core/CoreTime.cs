namespace EB
{
	// a time class that sync'd with the server (in UTC)
	public static class Time
	{
		private static int 				_offset = -1;
		private static System.DateTime 	_epoc = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		
		public static bool Valid
		{
			get { return _offset > 0; }
		}
		
		public static int Now
		{
			get
			{
				return GetNow();
			}
			set
			{	
				SetNow(value);
			}
		}
		
		private static int GetNow()
		{
			if ( !Valid )
			{
				return ToPosixTime(System.DateTime.UtcNow); 
			}
			try {
				return UnityEngine.Mathf.FloorToInt(UnityEngine.Time.realtimeSinceStartup) + _offset;	
			}
			catch {
				return ToPosixTime(System.DateTime.UtcNow); 
			}
		}
		
		public static int Since( System.DateTime dt )
		{
			var span = System.DateTime.Now - dt;
			var diff = (int)span.TotalSeconds;
			return UnityEngine.Mathf.Max(0, diff);
		}
		
        public static int After(int t)
        {
            return UnityEngine.Mathf.Max(0, t - Now);
        }
		public static int Since( int t )
		{
			return UnityEngine.Mathf.Max(0, Now-t);
		}

		public static int Since( double t )
		{
			return Since((int)t);
		}
		
		private static void SetNow(int value)
		{
			_offset = value - (int)UnityEngine.Mathf.FloorToInt(UnityEngine.Time.realtimeSinceStartup); 
		}

		private static void SetNow(double value)
		{
			SetNow((int)value);
		}

		public static int ToPosixTime( System.DateTime time )
		{
			var span = time - _epoc;
			return (int)span.TotalSeconds;
		}

		public static System.DateTime FromPosixTime(int time)
		{
			return _epoc + System.TimeSpan.FromSeconds(time);
		}

		public static System.DateTime FromPosixTime(double time)
		{
			return FromPosixTime((int)time);
		}

		public static bool IsToday(double timestamp)
		{
			System.DateTime baseTime = _epoc;
			System.TimeSpan ts = System.DateTime.UtcNow - baseTime;

			System.TimeSpan ts2 = System.TimeSpan.FromSeconds(timestamp);

			return System.Math.Abs(System.Math.Floor(ts.TotalDays) - System.Math.Floor(ts2.TotalDays)) == 0.0;
		}

		public static bool IsLocalToday(double utcTimestamp)
		{
			System.DateTime baseTime = _epoc;
			System.DateTime utcTime = baseTime.AddSeconds(utcTimestamp);
			System.DateTime localTime = System.TimeZone.CurrentTimeZone.ToLocalTime(utcTime);
			System.DateTime localNow = System.DateTime.Now;

			return localNow.ToShortDateString() == localTime.ToShortDateString();
		}

		public static float deltaTime {get { return UnityEngine.Time.deltaTime; }}
		public static float timeScale {get { return UnityEngine.Time.timeScale; } set { UnityEngine.Time.timeScale = value; } }
		public static float realtimeSinceStartup {get { return UnityEngine.Time.realtimeSinceStartup; }}

		#region for c# structure convenient
		public static System.DateTime LocalNow
		{
			get { return FromPosixTime(Now).ToLocalTime(); }
		}

		public static int LocalYear
		{
			get { return LocalNow.Year; }
		}

		public static int LocalMonth
		{
			get { return LocalNow.Month; }
		}

		public static int LocalDay
		{
			get { return LocalNow.Day; }
		}

		public static int LocalWeek
		{
			get { return (int)LocalNow.DayOfWeek; }
		}

		public static int LocalDaysInMonth
		{
			get { return System.DateTime.DaysInMonth(LocalYear, LocalMonth); }
		}

		public static System.TimeSpan LocalTimeOfDay
		{
			get { return LocalNow.TimeOfDay; }
		}

		public static bool LocalInRange(System.TimeSpan start, System.TimeSpan end)
		{
			System.TimeSpan now = LocalTimeOfDay;
			return now >= start && now <= end;
		}
		#endregion
	}
}

