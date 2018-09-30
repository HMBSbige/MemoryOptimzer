using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryOptimzer
{
	public class SystemInfo
	{
		private readonly PerformanceCounter _pcCpuLoad;   //CPU计数器

		private const int GW_HWNDFIRST = 0;
		private const int GW_HWNDNEXT = 2;
		private const int GWL_STYLE = (-16);
		private const int WS_VISIBLE = 268435456;
		private const int WS_BORDER = 8388608;

		#region AIP声明
		[DllImport(@"IpHlpApi.dll")]
		public static extern uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

		[DllImport(@"User32")]
		private static extern int GetWindow(int hWnd, int wCmd);

		[DllImport(@"User32")]
		private static extern int GetWindowLongA(int hWnd, int wIndx);

		[DllImport(@"user32.dll")]
		private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

		[DllImport(@"user32", CharSet = CharSet.Auto)]
		private static extern int GetWindowTextLength(IntPtr hWnd);
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造函数，初始化计数器等
		/// </summary>
		public SystemInfo()
		{
			//初始化CPU计数器
			_pcCpuLoad = new PerformanceCounter(@"Processor", @"% Processor Time", @"_Total")
			{
				MachineName = @"."
			};
			_pcCpuLoad.NextValue();

			//CPU个数
			ProcessorCount = Environment.ProcessorCount;

			//获得物理内存
			var mc = new ManagementClass(@"Win32_ComputerSystem");
			var moc = mc.GetInstances();
			foreach (var o in moc)
			{
				var mo = (ManagementObject) o;
				if (mo[@"TotalPhysicalMemory"] != null)
				{
					PhysicalMemory = long.Parse(mo[@"TotalPhysicalMemory"].ToString());
				}
			}
		}
		#endregion

		#region CPU个数
		/// <summary>
		/// 获取CPU个数
		/// </summary>
		public int ProcessorCount { get; } = 0;

		#endregion

		#region CPU占用率
		/// <summary>
		/// 获取CPU占用率
		/// </summary>
		public float CpuLoad => _pcCpuLoad.NextValue();

		#endregion

		#region 可用内存
		/// <summary>
		/// 获取可用内存
		/// </summary>
		public static long MemoryAvailable
		{
			get
			{
				long availablebytes = 0;
				var mos = new ManagementClass(@"Win32_OperatingSystem");
				foreach (var o in mos.GetInstances())
				{
					var mo = (ManagementObject) o;
					if (mo[@"FreePhysicalMemory"] != null)
					{
						availablebytes = 1024 * long.Parse(mo[@"FreePhysicalMemory"].ToString());
					}
				}
				return availablebytes;
			}
		}
		#endregion

		#region 物理内存
		/// <summary>
		/// 获取物理内存
		/// </summary>
		public long PhysicalMemory { get; } = 0;

		#endregion

		#region 结束指定进程
		/// <summary>
		/// 结束指定进程
		/// </summary>
		/// <param name="pid">进程的 Process ID</param>
		public static void EndProcess(int pid)
		{
			try
			{
				var process = Process.GetProcessById(pid);
				process.Kill();
			}
			catch
			{
				//ignore
			}
		}
		#endregion


		#region 查找所有应用程序标题
		/// <summary>
		/// 查找所有应用程序标题
		/// </summary>
		/// <returns>应用程序标题范型</returns>
		public static List<string> FindAllApps(int handle)
		{
			var apps = new List<string>();

			var hwCurr = GetWindow(handle, GW_HWNDFIRST);

			while (hwCurr > 0)
			{
				const int isTask = (WS_VISIBLE | WS_BORDER);
				var lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
				var taskWindow = (lngStyle & isTask) == isTask;
				if (taskWindow)
				{
					var length = GetWindowTextLength(new IntPtr(hwCurr));
					var sb = new StringBuilder(2 * length + 1);
					GetWindowText(hwCurr, sb, sb.Capacity);
					var strTitle = sb.ToString();
					if (!string.IsNullOrEmpty(strTitle))
					{
						apps.Add(strTitle);
					}
				}
				hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
			}

			return apps;
		}
		#endregion
	}
}