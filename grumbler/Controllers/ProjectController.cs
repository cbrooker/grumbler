using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace grumbler.Controllers
{
    public class ProjectController : Controller
    {
        dynamic _projects;
        public ProjectController() {
            _projects = new Projects();
        }
        public ActionResult Create() {
            return View();
        }
        [HttpPost]
        public ActionResult Create(string Name, string RepositoryUrl, string DeployPath) {
            var deployPath = DeployPath;
            if (String.IsNullOrEmpty(deployPath)) {
                var path = Path.Combine(Server.MapPath("~/App_Data"), Name);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                deployPath = path;
                
            }
            var project = new { Name = Name, RepositoryUrl = RepositoryUrl, DeployPath = deployPath };
            var newProject = _projects.Insert(project);
            return RedirectToAction("show", new { id = newProject.ID });
        }
        public ActionResult Show(int id) {
            dynamic p = _projects.First(ID: id);
            return View(p);
        }
        [HttpPost]
        public ActionResult Clone(int id) {
            var p = _projects.First(ID: id);
            //since we're cloning into the target - we need to run from one directory up
            //so get the parent
            var di = new DirectoryInfo(p.DeployPath);
            var deployParent = di.Parent.FullName;

            //when we clone - make sure to use the base directory name
            var dirName = di.Name;

            var cmd = string.Format("cd {0} && git clone {1} {2}", deployParent, p.RepositoryUrl, dirName);

            //execute
            var result = GitRunner.RunCommand(cmd);

            //clean up
            result += GitRunner.CleanUpClone(p.DeployPath);


            //set the latest commit as the deployed one
            var repo = new GitRepo(p.DeployPath);
            p.DeployedID= repo.CurrentCommit.Hash;
            _projects.Update(p, id);

            TempData["message"] = result;
            return RedirectToAction("show", new { id = id });

        }

        [HttpPost]
        public ActionResult Pull(int ID) {
            var project = _projects.First(ID: ID);
            var cmd = string.Format("cd {0} && git pull origin master", project.DeployPath.Replace("\\","/"));
            var result = GitRunner.RunCommand(cmd);
            TempData["message"] = result;
            return RedirectToAction("show", new { id = ID });

        }
        [HttpPost]
        public ActionResult Update(int ID, string Name, string RepositoryUrl, string DeployPath) {
            
            var p = _projects.First(ID:ID);
            //remove the old directory, if possible
            var sb = new StringBuilder();
            if (Directory.Exists(p.DeployPath)) {
                try {
                    Directory.Delete(p.DeployPath);
                    sb.AppendLine("Deleted old project path: "+p.DeployPath);
                } catch {
                    sb.AppendLine("Tried to delete old project path - couldn't do it "+p.DeployPath);
                }

            }
            p.DeployPath=DeployPath;
            _projects.Update(p, ID);
            sb.AppendLine("Updated project path to "+DeployPath);
            TempData["message"] = sb.ToString();

            return RedirectToAction("show", new { id = ID });
        }
        [HttpPost]
        public ActionResult Deploy(int id, string Hash) {
            var project=_projects.First(ID:id);
            var cmd = string.Format("cd {0} && git reset {1} --hard", project.DeployPath.Replace("\\", "/"), Hash);
            var result = GitRunner.RunCommand(cmd);
            project.DeployedID = Hash;
            _projects.Update(project, id);
            TempData["message"] = result;
            return RedirectToAction("show", new { id = id });
        }
    }
}
