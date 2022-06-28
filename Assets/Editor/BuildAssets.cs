using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory + "/Windows");
            Directory.CreateDirectory(assetBundleDirectory + "/Linux");
            Directory.CreateDirectory(assetBundleDirectory + "/MacOs");
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/Windows", 
                                        BuildAssetBundleOptions.UncompressedAssetBundle, 
                                        BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/Linux", 
                                        BuildAssetBundleOptions.UncompressedAssetBundle, 
                                        BuildTarget.StandaloneLinux64);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/MacOs", 
                                        BuildAssetBundleOptions.UncompressedAssetBundle, 
                                        BuildTarget.StandaloneOSX);
    }
    [MenuItem("Assets/Build AssetBundles(Windows)")]
    static void BuildAllAssetBundlesWindows()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory + "/Windows");
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory + "/Windows", 
                                        BuildAssetBundleOptions.UncompressedAssetBundle, 
                                        BuildTarget.StandaloneWindows);
    }
}