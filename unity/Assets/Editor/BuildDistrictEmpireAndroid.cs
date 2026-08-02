using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DistrictEmpire.EditorTools
{
    public static class BuildDistrictEmpireAndroid
    {
        private const string ScenePath = "Assets/DistrictEmpire/Presentation/Scenes/DistrictEmpireVerticalSlice.unity";
        private const string OutputPath = "Builds/DistrictEmpire-0.5.apk";

        [MenuItem("District Empire/Build Android APK")]
        public static void BuildApk()
        {
            if (!File.Exists(ScenePath)) SetupDistrictEmpireScene.Create();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            PlayerSettings.companyName = "DisorderMail";
            PlayerSettings.productName = "District Empire";
            PlayerSettings.applicationIdentifier = "com.disordermail.districtempire";
            PlayerSettings.bundleVersion = "0.5";
            PlayerSettings.Android.bundleVersionCode = 5;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.forceInternetPermission = true;
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

            if (report.summary.result != BuildResult.Succeeded)
                throw new System.Exception("Android APK build failed. Check the Unity build log for details.");

            Debug.Log("District Empire APK created: " + Path.GetFullPath(OutputPath));
        }
    }
}
