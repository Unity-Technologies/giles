using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace GILES
{
    public class pb_DirectoryMap
    {
        public string name;
        public string path;
        public List<pb_DirectoryMap> directories;
        public List<string> files;

        public pb_DirectoryMap(string name, string path)
        {
            this.name = name;
            this.path = path;
            map(path);
        }
        public List<string> getSubDirectoryNames()
        {
            List<string> names = new List<string>();
            foreach (pb_DirectoryMap dir in directories)
            {
                names.Add(dir.name);
            }
            return names;
        }

        public string getParrentDirectory()
        {
            Regex r = new Regex(".*\\/");
            Match m = r.Match(path);
            if (m.Success)
            {
                string parDirectory = m.Value;
                return parDirectory.Substring(0, parDirectory.Length - 1);
            }

            return "";
        }

        public string getRoot()
        {
            Regex r = new Regex(".*?\\/");
            Match m = r.Match(path);
            return m.Value;
        }

        public string getPathNoRoot()
        {
            string root = getRoot();
            if (root != "")
            {
                return path.Replace(root, "");
            }
            return "";
        }

        public List<string> getFileNames()
        {
            return files;
        }

        public List<string> getFileMatch(string pattern)
        {
            Regex r = new Regex(pattern);
            List<string> matches = new List<string>();
            foreach (string file in files)
            {
                if (r.IsMatch(file))
                {
                    matches.Add(file);
                }
            }
            return matches;
        }

        public void map(string path)
        {
            directories = new List<pb_DirectoryMap>();
            files = new List<string>();
            DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/Resources/" + path);
            FileInfo[] fileInfo = info.GetFiles();
            foreach (FileInfo fInfo in fileInfo)
            {
                if (isFolder(fInfo.Name))
                {
                    string folderName = fInfo.Name.Replace(".meta", "");
                    directories.Add(new pb_DirectoryMap(folderName, path + "/" + folderName));
                }
                else
                {
                    files.Add(fInfo.Name);

                }
            }

        }

        bool isFolder(string fName)
        {
            int sIndex = fName.LastIndexOf(".meta");
            if (sIndex == -1)
                return false;
            if ((fName.Substring(0, sIndex).IndexOf(".")) != -1)
                return false;

            return true;
        }
    }
}