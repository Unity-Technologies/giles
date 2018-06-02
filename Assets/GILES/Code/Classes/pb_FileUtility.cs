using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.IO.Compression;

namespace GILES
{
	/**
	 * Helper functions for reading information from files.
	 */
	public static class pb_FileUtility
	{
		/**
		 * Read all text from a file path.
		 */
		public static string ReadFile(string path)
		{
            string p = Path.GetFullPath(path);
            p = p.Replace(" ", "\\ ");
            p = p.Replace("%20", " ");
            if ( !File.Exists(p))
			{
				Debug.LogError("File path does not exist!\n" + p);
				return "";
			}

            string contents = Unzip(File.ReadAllBytes(p));

			return contents;
		}

		/**
		 * Save some text to a file path.  Does not check that folder structure is valid!
		 */
		public static bool SaveFile(string path, string contents)
		{
			try
			{
                string p = Path.GetFullPath(path);
                p = p.Replace(" ", "\\ ");
                p = p.Replace("%20", " ");
                File.WriteAllBytes(p, Zip(contents));
			} 
			catch(System.Exception e)
			{
				Debug.LogError("Failed writing to path: " + path + "\n" + e.ToString());
				return false;
			}

			return true;
		}

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static bool IsValidPath(string path, string extension)
		{
			return !string.IsNullOrEmpty(path) && 
				System.Uri.IsWellFormedUriString(path, System.UriKind.RelativeOrAbsolute) && 
				path.EndsWith(extension);
		}

		/**
		 * Given a string, this function attempts to extrapolate the absolute path using current directory as the
		 * relative root.
		 */
		public static string GetFullPath(string path)
		{
			string full = Path.GetFullPath(path);
			return full;
		}

		/**
		 * Return the path type (file or directory)
		 */
		public static PathType GetPathType(string path)
		{
			return File.Exists(path) ? PathType.File : (Directory.Exists(path) ? PathType.Directory : PathType.Null);
		}

		/**
		 * Replace backslashes with forward slashes, and make sure that path is the full path.
		 */
		public static string SanitizePath(string path, string extension = null)
		{
			string rep = GetFullPath(path);
			// @todo On Windows this defaults to '\', but doesn't escape correctly.
			// Path.DirectorySeparatorChar.ToString());
			rep = Regex.Replace(rep, "(\\\\|\\\\\\\\){1,2}|(/)", "/");
			// white space gets the escaped symbol
			rep = Regex.Replace(rep, "\\s", "%20");

			if(extension != null && !rep.EndsWith(extension))
			{
				if(!extension.StartsWith("."))
					extension = "." + extension;

				rep += extension;
			}

			return rep;
		}
	}
}
