using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GitSharp.Core.Transport;
using System.IO;
using System.Text;
using GitSharp.Core;

    public class GitRepo {
        public GitRepo(string repoPath) {
            RepoPath = repoPath.Trim('\\');
        }

        public string Name { get; set; }
        public string RepoPath { get; set; }
        public string URL { get; set; }
        public string CommitHash(GitSharp.Commit c) {
            if (c == null)
                return "";
            return c.Hash.Substring(0, 8);
        }
        public IEnumerable<GitSharp.Commit> LastEight() {
            return CurrentCommit.Ancestors.Take(8);
        }
        public bool IsRepo() {
            if (IsLocal) {
                if (!Directory.Exists(RepoPath))
                    return false;
                return this.RepoDirectory.GetDirectories().Any(x => x.Name == ".git");
            }
            //assumption is that it's a Github repo or something...
            return true;
        }
        public bool IsLocal {
            get {
                return !RepoPath.StartsWith("git:") & !RepoPath.StartsWith("http:") & !RepoPath.StartsWith("git@"); ;
            }
        }
        public string HEAD {
            get {
                return Path.Combine(ProperGitPath, "HEAD");
            }
        }
        public GitSharp.Commit CurrentCommit {
            get {
                return this.Repository.CurrentBranch.CurrentCommit;
            }
        }
        public GitSharp.Commit PreviousCommit {
            get {
                if (Repository != null)
                    return this.Repository.CurrentBranch.CurrentCommit.Ancestors.Skip(1).First();
                return null;
            }
        }
        public GitSharp.Repository Repository {
            get {
                if (Directory.Exists(this.ProperGitPath))
                    return new GitSharp.Repository(this.ProperGitPath);
                return null;
            }
        }

        public string ProperGitPath {
            get {

                if (!RepoPath.EndsWith(".git"))
                    return RepoPath + "\\.git";
                return RepoPath;
            }
        }
        public DirectoryInfo RepoDirectory {
            get {
                return new DirectoryInfo(RepoPath);
            }
        }
}