using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace CostCenter.Editor {
    public class CCBuildPostProcessor
    {

        [PostProcessBuildAttribute(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                // Read.
                string projectPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromString(File.ReadAllText(projectPath));
                string targetGUID = project.TargetGuidByName("UnityFramework");

                AddFrameworks(project, targetGUID);

                // Write.
                File.WriteAllText(projectPath, project.WriteToString());
            }
        }

        static void AddFrameworks(PBXProject project, string targetGUID)
        {
            project.AddFrameworkToProject(targetGUID, "AdServices.framework", false);

            // Add `-ObjC` to "Other Linker Flags".
            // project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");

            // Disable bitcode
            // project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "false");
        }

    }
}
