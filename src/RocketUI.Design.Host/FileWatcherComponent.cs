using System;
using System.IO;
using Microsoft.Xna.Framework;
using NLog;

namespace RocketUI.Design.Host
{
    public class FileWatcherComponent : GameComponent
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        private readonly Action<string> _onFileUpdate;
        public           string         FilePath { get; }

        private readonly FileSystemWatcher _fileSystemWatcher;
        
        public FileWatcherComponent(Game game, string filePath, Action<string> onFileUpdate) : base(game)
        {
            _onFileUpdate = onFileUpdate;
            FilePath = filePath;
            
            Log.Debug("Setting up File listener for {0}", FilePath);
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = Path.GetFullPath(Path.GetDirectoryName(FilePath));
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }
        
        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            Log.Debug("Detected file {1}: {0} ({2})", e.Name, e.ChangeType.ToString(), e.FullPath);
            if (e.Name == Path.GetFileName(FilePath))
            {
                _onFileUpdate.Invoke(e.FullPath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _fileSystemWatcher?.Dispose();
            base.Dispose(disposing);
        }
    }
}