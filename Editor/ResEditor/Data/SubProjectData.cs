using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Framework
{
    public class SubProjectData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Folder { get; set; }
        
        public List<AssetBundleBuild> Builds = new List<AssetBundleBuild>();


        public static List<SubProjectData> SearchAllInProject()
        {
            return AssetDatabase.GetAllAssetPaths().Where(assetPath => assetPath.EndsWith(".asset"))
                .Select(assetPath =>
                {
                    var subProject = AssetDatabase.LoadAssetAtPath<SubProject>(assetPath);

                    if (subProject)
                    {
                        return new SubProjectData()
                        {
                            Path = assetPath,
                            Folder = assetPath.RemoveString(subProject.name + ".asset"),
                            Name = subProject.name,
                        };
                    }

                    return null;
                })
                .Where(data => data != null)
                .ToList();
        }

        public static void SplitAssetBundles2DefaultAndSubProjectDatas(SubProjectData defaultSubProjectData, List<SubProjectData> subProjectDatas)
        {
            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            foreach (var assetBundleName in assetBundleNames)
            {
                var assetBundleBuild = new AssetBundleBuild
                {
                    assetBundleName = assetBundleName,
                    assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName)
                };
                var isDefault = true;
                  
                
                foreach (var subProjectData in subProjectDatas)
                {
                    foreach (var assetName in assetBundleBuild.assetNames)
                    {
                        if (assetName.Contains(subProjectData.Folder))
                        {
                            subProjectData.Builds.Add(assetBundleBuild);
                            isDefault = false;
                            break;
                        }
                    }
                }

                if (isDefault)
                {
                    defaultSubProjectData.Builds.Add(assetBundleBuild);
                }
            }
        }
    }

}