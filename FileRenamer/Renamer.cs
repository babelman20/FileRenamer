using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections;

namespace FileRenamer {
    public class Renamer {
        private readonly FileSystemWatcher[] watchers;

        public Renamer() {
            string[] paths = ConfigurationManager.AppSettings["FilePath"].Split(';');
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", "Watching " + paths.Length + " directories\n");

            foreach (string path in paths) {
                foreach (string subdir in Directory.GetDirectories(path)) {
                    foreach (string file in Directory.GetFiles(subdir)) {
                        CheckName(file);
                    }
                }
                foreach (string file in Directory.GetFiles(path)) {
                    CheckName(file);
                }
            }

            watchers = new FileSystemWatcher[paths.Length];
            for (int i = 0; i < paths.Length; i++) {
                FileSystemWatcher watcher = new FileSystemWatcher(@paths[i]);
                watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess |
                                        NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security;
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnCreated);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);
                watcher.Error += new ErrorEventHandler(OnError);
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                watchers[i] = watcher;
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e) {
            if (e.ChangeType != WatcherChangeTypes.Changed) {
                return;
            }
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", "Found " + e.FullPath + "\n");

            //Start rename
            FileSystemWatcher watcher = (FileSystemWatcher) sender;
            watcher.EnableRaisingEvents = false;
            CheckName(e.FullPath);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnCreated(object sender, FileSystemEventArgs e) {
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", "Found " + e.FullPath + "\n");

            //Start rename
            FileSystemWatcher watcher = (FileSystemWatcher)sender;
            watcher.EnableRaisingEvents = false;
            CheckName(e.FullPath);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnRenamed(object sender, FileSystemEventArgs e) {
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", "Found " + e.FullPath + "\n");

            //Start rename
            FileSystemWatcher watcher = (FileSystemWatcher)sender;
            watcher.EnableRaisingEvents = false;
            CheckName(e.FullPath);
            watcher.EnableRaisingEvents = true;
        }

        private static void CheckName(string path) {
            if (!File.Exists(path)) {
                return;
            }

            string name = Path.GetFileNameWithoutExtension(path);
            StringBuilder sb = new StringBuilder();
            foreach (char c in name) {
                if (Char.IsLetterOrDigit(c) || c == '_') {
                    sb.Append(c);
                }
            }

            string newPath = Path.Combine(Path.GetDirectoryName(path), sb.ToString()) + Path.GetExtension(path);
            if (name.CompareTo(sb.ToString()) != 0) {
                if (File.Exists(newPath)) {
                    int ct = 1;
                    string fixPath = Path.Combine(Path.GetDirectoryName(newPath), sb.ToString()) + "(" + ct + ")" + Path.GetExtension(newPath);
                    while (File.Exists(fixPath)) {
                        ct++;
                        fixPath = Path.Combine(Path.GetDirectoryName(newPath), sb.ToString()) + "(" + ct + ")" + Path.GetExtension(newPath);
                    }
                    newPath = fixPath;
                }
                File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", "File: " + path + " renamed to " + newPath + "\n");
                File.Move(path, newPath);
            }
        }

        private static void OnError(object sender, ErrorEventArgs e) {
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", e.GetException().ToString() + "\n");
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e) {
            File.AppendAllText(@"C:\Temp\Demos\FileRename.txt", DateTime.Now.ToString() + "\n");
        }

        public void Start() {
            //Do nothing on start
        }

        public void Stop() {
            //watcher.Dispose();
        }
    }
}
