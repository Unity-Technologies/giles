using UnityEngine;
using System.Collections;
using GILES.Interface;

namespace GILES.Example
{
	/**
	 * Opens a file browser to select a level, then plays that level
	 */
	public class LoadAndPlayScene : MonoBehaviour
	{
		public pb_FileDialog dialogPrefab;

		/**
		 * Open the load dialog.
		 */
		public void OpenLoadPanel()
		{
			pb_FileDialog dlog = GameObject.Instantiate(dialogPrefab);
			dlog.SetDirectory(System.IO.Directory.GetCurrentDirectory());
			dlog.isFileBrowser = true;
			dlog.filePattern = "*.json";
			dlog.AddOnSaveListener(OnOpen);

			pb_ModalWindow.SetContent(dlog.gameObject);
			pb_ModalWindow.SetTitle("Open Scene");
			pb_ModalWindow.Show();
		}

		private void OnOpen(string path)
		{
			pb_SceneLoader.LoadScene(path);
		}
	}
}
