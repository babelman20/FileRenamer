using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FileRenamer {
    class Program {
        static void Main(string[] args) {
            var exitCode = HostFactory.Run(x => {
                x.Service<Renamer>(s => {
                    s.ConstructUsing(renamer => new Renamer());
                    s.WhenStarted(renamer => renamer.Start());
                    s.WhenStopped(renamer => renamer.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("FileRenameService");
                x.SetDisplayName("File Renamer Service");
                x.SetDescription("This is a service which elminiates special characters from the given filepath.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
