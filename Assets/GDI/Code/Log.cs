using System;
using UnityEngine;

namespace Assets.GDI.Code
{
	/// <summary>
	/// A static class to log console messages.
	/// </summary>
	public static class Log
	{

		public static void Info(String info)
		{
			if (Config.LogLevel > 0)
			{
				Debug.Log(info);
			}
		}

		public static void Error(String error)
		{
			Debug.LogError(error);
		}

	}


}

