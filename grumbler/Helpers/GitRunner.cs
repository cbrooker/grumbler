using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Diagnostics;

    public static class GitRunner {
        public static string RunCommand(string command) {
            ProcessStartInfo s = new ProcessStartInfo("cmd.exe");
            s.CreateNoWindow = true;
            s.ErrorDialog = false;
            s.RedirectStandardOutput = true;
            s.UseShellExecute = false;
            s.RedirectStandardInput = true;
            s.RedirectStandardOutput = true;
            s.RedirectStandardError = true;
            var result = "";
            using (Process p = Process.Start(s)) {
                System.IO.StreamWriter SW = p.StandardInput;
                SW.WriteLine(command);
                SW.Close();
                SW.Dispose();
                p.WaitForExit();
                result = p.StandardOutput.ReadToEnd();
            }
            return result;
        }
        public static bool IsGitRepo(string dirPath) {
            if (!Directory.Exists(dirPath))
                return false;
            var dir = new DirectoryInfo(dirPath);
            return dir.GetDirectories().Any(x => x.Name == ".git");
        }

        public static string CleanUpClone(string dirPath) {
            var sb = new StringBuilder();
            //there are number of files that are included here that we'll need to remove....
            var dir = new DirectoryInfo(dirPath);
            sb.AppendLine("Cleaning up " + dir.FullName);

            //remove all MVC Code stuff
            sb.AppendLine(DeleteIfExists(dir, "Models"));
            sb.AppendLine(DeleteIfExists(dir, "Controllers"));
            sb.AppendLine(DeleteIfExists(dir, "Areas"));
            sb.AppendLine(DeleteIfExists(dir, "Infrastructure"));
            sb.AppendLine(DeleteIfExists(dir, "Properties"));

            //remove the code files
            foreach (var file in GetFilesRecursive(dir, "*.cs")) {
                file.Delete();
                sb.AppendLine("Deleted " + file.Name);
            }

            return sb.ToString();
        }
        //http://weblogs.asp.net/whaggard/archive/2004/08/15/214864.aspx
        public static IEnumerable<FileInfo> GetFilesRecursive(DirectoryInfo dirInfo) {
            return GetFilesRecursive(dirInfo, "*.*");
        }
        public static IEnumerable<FileInfo> GetFilesRecursive(DirectoryInfo dirInfo, string searchPattern) {
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                foreach (FileInfo fi in GetFilesRecursive(di, searchPattern))
                    yield return fi;

            foreach (FileInfo fi in dirInfo.GetFiles(searchPattern))
                yield return fi;
        }
        public static string DeleteIfExists(DirectoryInfo root, string sub) {
            if (root.GetDirectories().Any(x => x.Name.Equals(sub, System.StringComparison.OrdinalIgnoreCase))) {
                root.GetDirectories(sub).First().Delete();
                return "Deleted " + sub;
            }
            return " -- " + sub + " directory not present";
        }
    }
