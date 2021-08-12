using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RocketUI.Editor.Utilities
{
    public class VueDevServerWrapper : IDisposable
    {
        private ProcessStartInfo _startInfo;
            private Process          _process;

            internal VueDevServerWrapper(ushort port)
            {
                var exe  = "npm";
                var args = $"run serve -- --port {port}";

                if (OperatingSystem.IsWindows())
                {
                    args = $"/c {exe} {args}";
                    exe = "cmd";
                }

                var startInfo = new ProcessStartInfo(exe);

                startInfo.Arguments = args;
                startInfo.WorkingDirectory = Path.Join(FindProjectDirectory(), "vueapp");
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                _startInfo = startInfo;
            }

            private string FindProjectDirectory()
            {
                var dir   = Directory.GetCurrentDirectory();
                var found = false;
                while (!found)
                {
                    var di = Directory.GetParent(dir);
                    if (di == null)
                    {
                        return null;
                    }
                    
                    dir = di?.FullName;
                    if (di.GetFileSystemInfos("vueapp").Any(f => f.Name.Equals("vueapp", StringComparison.Ordinal)))
                    {
                        found = true;
                    }
                }
                
                return dir;
            }

            internal void Start()
            {
                if (_process != null) return;

                _process = Process.Start(_startInfo);
                _process.EnableRaisingEvents = true;
                _process.OutputDataReceived += new DataReceivedEventHandler(StandardOutputHandler);
                _process.BeginOutputReadLine();
                _process.ErrorDataReceived += new DataReceivedEventHandler(StandardErrorHandler);
                _process.BeginErrorReadLine();
            }

            internal void Stop()
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                    _process = null;
                }
            }

            public static void StandardOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
            {
                if (!string.IsNullOrEmpty(outLine.Data))
                {
                    Console.WriteLine($"VUE|INFO> {outLine.Data}");
                }
            }

            public static void StandardErrorHandler(object sendingProcess, DataReceivedEventArgs errorLine)
            {
                if (!string.IsNullOrEmpty(errorLine.Data))
                {
                    Console.WriteLine($"VUE|ERROR> {errorLine.Data}");
                }
            }

            public void Dispose()
            {
                Stop();
            }
    }
}