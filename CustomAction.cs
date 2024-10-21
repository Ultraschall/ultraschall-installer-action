using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Deployment.WindowsInstaller;

namespace custom_action
{
  public class CustomActions
  {
    [CustomAction]
    public static ActionResult PreInstallAction(Session session)
    {
      session.Log("ULTRASCHALL: Begin PreInstallAction");
      try {
        var profileFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var sourceFolder = profileFolder + @"\REAPER";
        // check for installed REAPER profile
        if (Directory.Exists(sourceFolder) == true) {
          var targetFolder = profileFolder + @"\Ultraschall\Backups\" + CreateTimestampString() + @"\REAPER";
          MoveDirectoryPreserveLicense(session, sourceFolder, targetFolder);
        }
        else {
          // nothing to do, bail out...
          session.Log("ULTRASCHALL: No backup required. Done.");
        }

        session.Log("ULTRASCHALL: End PreInstallAction");
      }
      catch (Exception e) {
        session.Log("ULTRASCHALL: Internal error (PreInstallAction)");
        session.Log(e.Message);
        session.Log(e.StackTrace);
        return ActionResult.Failure;
      }
      return ActionResult.Success;
    }

    private static void MoveDirectoryPreserveLicense(Session session, String source, String target)
    {
      try {
        if (Directory.Exists(target) == false) {
          Directory.CreateDirectory(target);
          session.Log("ULTRASCHALL: " + target + " created");
        }

        var files = Directory.GetFiles(source);
        var directories = Directory.GetDirectories(source);

        foreach (var directory in directories) {
          session.Log("ULTRASCHALL: " + directory + " found");
          MoveDirectoryPreserveLicense(session, Path.Combine(source, Path.GetFileName(directory)), Path.Combine(target, Path.GetFileName(directory)));
          session.Log("ULTRASCHALL: " + directory + " copied");
          if (PreserveDirectory(directory) == false) 
          {
            Directory.Delete(directory);
          }
          session.Log("ULTRASCHALL: " + directory + " deleted");
        }

        foreach (var file in files) {
          session.Log("ULTRASCHALL: " + file + " found");
          File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
          session.Log("ULTRASCHALL: " + file + " copied");
          if (String.Equals(Path.GetFileName(file), "reaper-license.rk", StringComparison.OrdinalIgnoreCase) == false) {
            if(PreserveFile(file) == false)
            {
              File.Delete(file);
            }
            session.Log("ULTRASCHALL: " + file + " deleted");
          }
          else {
            session.Log("ULTRASCHALL: " + file + " preserved");
          }
        }
      }
      catch (Exception e) {
        session.Log("ULTRASCHALL: Internal error (MoveDirectoryPreserveLicense)");
        session.Log(e.Message);
        session.Log(e.StackTrace);
      }
    }

    private static bool PreserveDirectory(string directory) 
    {
      if(directory.ToLower().Contains("projecttemplates")) 
      {
        return true;
      }
      return false;
    }

    private static bool PreserveFile(string file)
    {
      if (file.ToLower().Contains("projecttemplates")) 
      {
        if (file.ToLower().Contains("studiolink and soundboard.rpp")) {
          return false;
        }
        else if (file.ToLower().Contains("studiolink.rpp")) {
          return false;
        }
        else if (file.ToLower().Contains("ultraschall soundboard.rpp")) {
          return false;
        }
        else 
        {
          // Preserve custom templates
          return true;
        }
      }
      return false;
    }

    private static string CreateTimestampString()
    {
      var timestamp = DateTime.Now.ToLocalTime();
      var builder = new StringBuilder();
      builder.Append(timestamp.ToString("yyyyMMdd")).Append("T").Append(timestamp.ToString("Hmmss"));
      return builder.ToString();
    }
  }
}
