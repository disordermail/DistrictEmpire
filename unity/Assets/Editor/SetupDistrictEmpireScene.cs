using DistrictEmpire.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace DistrictEmpire.EditorTools
{
    public static class SetupDistrictEmpireScene
    {
        private const string ScenePath = "Assets/DistrictEmpire/Presentation/Scenes/DistrictEmpireVerticalSlice.unity";
        private const string PanelSettingsPath = "Assets/DistrictEmpire/Presentation/UI/DistrictEmpirePanelSettings.asset";

        [MenuItem("District Empire/Setup Vertical Slice Scene")]
        public static void Create()
        {
            EnsureFolder("Assets/DistrictEmpire/Presentation/Scenes");
            EnsureFolder("Assets/DistrictEmpire/Presentation/UI");

            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, PanelSettingsPath);
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var app = new GameObject("District Empire App");
            var document = app.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            document.sortingOrder = 0;
            app.AddComponent<DistrictEmpireBootstrap>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("District Empire Unity vertical slice scene created.");
        }

        private static void EnsureFolder(string assetPath)
        {
            var parts = assetPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
