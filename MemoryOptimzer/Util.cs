namespace MemoryOptimzer
{
	public static class Util
	{
		public static string CountSize(long size)
		{
			var mStrSize = string.Empty;
			const double step = 1024.00;
			var factSize = size;
			if (factSize < step)
			{
				mStrSize = $@"{factSize:F2} Byte";
			}
			else if (factSize >= step && factSize < 1048576)
			{
				mStrSize = $@"{factSize / step:F2} KB";
			}
			else if (factSize >= 1048576 && factSize < 1073741824)
			{
				mStrSize = $@"{factSize / step / step:F2} MB";
			}
			else if (factSize >= 1073741824 && factSize < 1099511627776)
			{
				mStrSize = $@"{factSize / step / step / step:F2} GB";
			}
			else if (factSize >= 1099511627776)
			{
				mStrSize = $@"{factSize / step / step / step / step:F2} TB";
			}

			return mStrSize;
		}
	}
}
