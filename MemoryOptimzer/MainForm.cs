using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryOptimzer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private delegate void VoidMethod_Delegate();

		[DllImport(@"psapi.dll")]
		private static extern int EmptyWorkingSet(IntPtr hwProc);

		private void ClearMemory()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			var processes = Process.GetProcesses();
			SetprogressBar2Max(processes.Length);
			var succeed = 0;
			foreach (var process in processes)
			{
				try
				{
					if (process.ProcessName == @"System" && process.ProcessName == @"Idle")
					{
						continue;
					}

					EmptyWorkingSet(process.Handle);
					++succeed;
				}
				catch
				{
					//ignore
				}
				finally
				{
					StepprogressBar2();
					SetLabel3($@"成功负优化：{succeed}/{processes.Length}");
				}
			}
		}

		private void SetprogressBar1(int value)
		{
			progressBar1.Invoke(new VoidMethod_Delegate(() =>
			{
				progressBar1.Value = value;
			}));
		}

		private void StepprogressBar2()
		{
			progressBar2.Invoke(new VoidMethod_Delegate(() =>
			{
				progressBar2.PerformStep();
			}));
		}

		private void SetprogressBar2Max(int value)
		{
			progressBar2.Invoke(new VoidMethod_Delegate(() =>
			{
				progressBar2.Value = 0;
				progressBar2.Maximum = value;
			}));
		}

		private void SetLabel3(string str)
		{
			label3.Invoke(new VoidMethod_Delegate(() =>
			{
				label3.Text = str;
			}));
		}

		private void SetLabel1(string str)
		{
			label1.Invoke(new VoidMethod_Delegate(() =>
			{
				label1.Text = str;
			}));
		}

		private void button1_Click(object sender, EventArgs e)
		{
			button1.Enabled = false;
			var t = new Task(ClearMemory);
			t.Start();
			t.ContinueWith(task =>
			{
				button1.BeginInvoke(new VoidMethod_Delegate(() =>
				{
					button1.Enabled = true;
				}));
			});
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Task.Run(() =>
			{
				var info = new SystemInfo();
				var memoryleft = SystemInfo.MemoryAvailable;
				var memory = info.PhysicalMemory;
				var memoryload = 1 - Convert.ToDouble(memoryleft) / memory;
				SetLabel1($@"内存使用：{Util.CountSize(memory - memoryleft)}/{Util.CountSize(memory)}");
				SetprogressBar1(Convert.ToInt32(100 * memoryload));
			});
		}
	}
}
