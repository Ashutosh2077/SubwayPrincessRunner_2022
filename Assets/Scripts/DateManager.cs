using System;
using System.Text;
using UnityEngine;

public class DateManager
{
	public static double CalcDeltaTime(long timeL)
	{
		return DateManager.CalcDeltaTime(DateManager.TranslateServeTicksToDateTime(timeL));
	}

	public static double CalcDeltaTime(DateTime now)
	{
		DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
		return (dateTime.AddDays(1.0) - now).TotalSeconds;
	}

	public static string CalcWeekRankString(long time)
	{
		DateTime now = DateManager.TranslateServeTicksToDateTime(time);
		return DateManager.CalcWeekRankString(now);
	}

	public static string CalcWeekRankString(DateTime now)
	{
		StringBuilder stringBuilder = new StringBuilder();
		DateTime dateTime = new DateTime(now.Year + 1, 1, 1);
		DateTime d = dateTime.AddDays(-1.0);
		DayOfWeek dayOfWeek = d.DayOfWeek;
		if ((d - now).Days < (int)dayOfWeek)
		{
			stringBuilder.Append(now.Year + 1);
			stringBuilder.Append("01");
			return stringBuilder.ToString();
		}
		stringBuilder.Append(now.Year);
		stringBuilder.AppendFormat("{0:D2}", DateManager.CalcWeekOfYear(now, false));
		return stringBuilder.ToString();
	}

	private static int CalcWeekOfYear(DateTime date, bool fremdness)
	{
		DayOfWeek dayOfWeek = new DateTime(date.Year, 1, 1).DayOfWeek;
		int num = 0;
		num = ((!fremdness) ? (date.DayOfYear + (int)(dayOfWeek + 6) % 7) : ((int)(date.DayOfYear + dayOfWeek)));
		int num2 = num / 7;
		if (num % 7 > 0)
		{
			num2++;
		}
		return num2;
	}

	public static DateTime TranslateServeTicksToDateTime(long ticks)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
		return dateTime.AddSeconds((double)ticks);
	}

	private static TimeSpan CalcRemainTimeToWeekend(DateTime date, bool fremdness)
	{
		int dayOfWeek = (int)date.DayOfWeek;
		int num;
		if (fremdness)
		{
			num = 7 - dayOfWeek;
		}
		else
		{
			num = 7 - (dayOfWeek + 6) % 7;
		}
		DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
		return dateTime.AddDays((double)num) - date;
	}

	public static TimeSpan CalcRemainTimeToWeekend(long ticks, float time)
	{
		return DateManager.CalcRemainTimeToWeekend(DateManager.TranslateServeTicksToDateTime(ticks).AddSeconds((double)(RealTimeTracker.time - time)), false);
	}

	public static string TopRunRemainTimeToString(long ticks, float time)
	{
		TimeSpan timeSpan = DateManager.CalcRemainTimeToWeekend(ticks, time);
		if (timeSpan.Seconds < 0 || timeSpan.Minutes < 0 || timeSpan.Hours < 0 || timeSpan.Days < 0)
		{
			return string.Empty;
		}
		return string.Format("{0}:{1:D2}:{2:D2} {3}", new object[]
		{
			timeSpan.Days * 24 + timeSpan.Hours,
			timeSpan.Minutes,
			timeSpan.Seconds,
			Strings.Get(LanguageKey.UI_SCREEN_RANK_LEFT)
		});
	}
}
