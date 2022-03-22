using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RocketUI.Utilities.IO
{
	public class XClipClipboard : IClipboardImplementation
	{
		private static string Run(string commandLine, bool waitForExit = true)
		{
			var errorBuilder = new StringBuilder();
			var outputBuilder = new StringBuilder();
			var arguments = $"-c \"{commandLine}\"";
			using (var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "bash",
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = false,
				}
			})
			{
				process.OutputDataReceived += (sender, args) => { outputBuilder.AppendLine(args.Data); };
				process.ErrorDataReceived += (sender, args) => { errorBuilder.AppendLine(args.Data); };
				process.Start();
                
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
                
				if (waitForExit)
				{
					if (!DoubleWaitForExit(process))
					{
						var timeoutError = $@"Process timed out. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
						throw new Exception(timeoutError);
					}
				}
				else
				{
					process.WaitForExit(500);
				}

				if (process.ExitCode == 0)
				{
					return outputBuilder.ToString();
				}

				var error = $@"Could not execute process. Command line: bash {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
				throw new Exception(error);
			}
		}

		//To work around https://github.com/dotnet/runtime/issues/27128
		static bool DoubleWaitForExit(Process process)
		{
			var result = process.WaitForExit(500);
			if (result)
			{
				process.WaitForExit();
			}
			return result;
		}

		public static bool IsXClipAvailable()
		{
			try
			{
				return false;
				
			}
			catch(Exception ex)
			{
				return false;
			}
		}
        
		public void SetText(string value)
		{
			var tempFileName = Path.GetTempFileName();
			File.WriteAllText(tempFileName, value);
			try
			{
				Run($"cat {tempFileName} | xclip -i -selection clipboard", false);
			}
			finally
			{
				File.Delete(tempFileName);
			}
		}

		public string GetText()
		{
			var tempFileName = Path.GetTempFileName();
			try
			{
				Run($"xclip -o -selection clipboard > {tempFileName}");
				return File.ReadAllText(tempFileName);
			}
			finally
			{
				File.Delete(tempFileName);
			}
		}
	}
}