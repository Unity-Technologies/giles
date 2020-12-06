using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using GILES;

namespace GILES.Interface
{
	public class pb_PrefabBrowser : MonoBehaviour
	{
		List<GameObject> prefabs = new List<GameObject>();

		void Start()
		{
            openFolder("");
			
		}

        private string cleanPath(string p)
        {
            p = p.Replace("//", "/");
            if (p.IndexOf('/') == 0)
                p = p.Substring(1);
            if (p == "/")
                return "";
            if (p.EndsWith("/"))
                p = p.Substring(0, p.Length - 1);

            return p;

        }

        private void createHomeIcons()
        {
            foreach (string folder in pb_Config.Resource_Folder_Paths)
            {
                createFolderButton("", folder);
            }
            foreach (string bundle in pb_Config.AssetBundle_Names)
            {
                createAssetBundleButton(bundle);
            }
        }
        private void createAssetBundleButton(string bundleName)
        {
            GameObject icon = transform.gameObject.AddChild();
            
        }
        private void createFolderButton(string path, string dir, string dirName = "")
        {
            GameObject icon = transform.gameObject.AddChild();
            pb_PrefabBrowserItemFolderIconButton button = icon.AddComponent<pb_PrefabBrowserItemFolderIconButton>();
            button.asset = Resources.Load<GameObject>(pb_Config.Default_Folder_Path + "/" + pb_Config.Default_Folder_Thumbnail);
            if (button.asset == null)
            {
                button.asset = new GameObject();
            }
            button.asset.name = (dirName != "") ? dirName : dir;
            button.path = path + "/" + dir;
            button.path = cleanPath(button.path);
            button.browser = this;
            button.Initialize();
        }
        private void LoadFileToPrefab(string fileName, string fPAth, string root)
        {
            GameObject filePrefab;
            if (root == "")
            {
                filePrefab = pb_ResourceManager.Load<GameObject>(fileName.Replace(".prefab", ""));
            }
            else
            {
                filePrefab = pb_ResourceManager.Load<GameObject>(fPAth + "/" + fileName.Replace(".prefab", ""));
            }
            if (filePrefab != null)
                prefabs.Add(filePrefab);
        }

        private void CreateFileButton(GameObject go)
        {
            GameObject icon = transform.gameObject.AddChild();
            pb_PrefabBrowserItemGameObjectsButton button = icon.AddComponent<pb_PrefabBrowserItemGameObjectsButton>();

            button.asset = go;
            button.Initialize();
        }

        private void clearBrowser()
        {
            foreach(Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Resources.UnloadUnusedAssets();
        }

        public void openAssetBundle(string bundleName)
        {
            clearBrowser();
            createFolderButton("", "", "../");
            List<GameObject> assets = new List<GameObject>();
            AssetBundle bundle = pb_AssetBundles.LoadAssetBundleWithName(bundleName);
            assets.AddRange(bundle.LoadAllAssets<GameObject>());
            foreach (GameObject go in assets)
            {
                CreateFileButton(go);
            }
        }

        public void openFolder (string path)
        {
            clearBrowser();
            pb_DirectoryMap dMap = new pb_DirectoryMap(path, path);
            prefabs = new List<GameObject>();
            List<string> fileNames = dMap.getFileMatch("\\.prefab");

            if (path == "")
                createHomeIcons();
            else
            {
                createFolderButton(dMap.getParrentDirectory(), "", "../");

                foreach (string dir in dMap.getSubDirectoryNames())
                {
                    createFolderButton(path, dir);
                }
                foreach(string file in fileNames)
                {
                    LoadFileToPrefab(file, dMap.getPathNoRoot(), dMap.getRoot());
                }
                foreach (GameObject go in prefabs)
                {
                    CreateFileButton(go);
                }
            }
#if UNITY_2019
        }
	}
}