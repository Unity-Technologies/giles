using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GILES;
using System.IO;

namespace GILES.Interface
{
	/**
	 * Implements a navigable directory window.
	 * \sa pb_Window, pb_ModalWindow
	 */
	public class pb_FileDialog : MonoBehaviour 
	{
		/// Store the history of the back and forward buttons
		private Stack<string> back = new Stack<string>();
		private Stack<string> forward = new Stack<string>();

		/// Where to put current directory folder buttons.
		public GameObject scrollContent;

		/// Save and cancel buttons.  `onClick` delegates will automatically be added by this script.
		public Button saveButton, cancelButton;

		/// The input field that shows the directory path.
		public InputField directoryCrumbsField;

		/// The input field that allows user to type in file or folder name
		public InputField fileInputField;

		/// The directory currently being inspected
		public string currentDirectory;

		/// Buttons to navigate folder structures.
		public Button backButton, forwardButton, upButton;

		/// The prefab to populate scrollview contents with
		public pb_SaveDialogButton rowButtonPrefab;

		/// pb_GUIStyle to apply to odd and even rows.
		public pb_GUIStyle oddRowStyle, evenRowStyle;

		/// If true, files as well as folders will be displayed.  If false, only folders will be
		/// shown.  This also affects the string returned by `OnSave` callback.
		public bool isFileBrowser { get { return _isFileBrowser; } set { _isFileBrowser = value; UpdateDirectoryContents(); } }

		private bool _isFileBrowser = false;

		/// If `isFileBrowser` is true, this string my be used to filter file results (see https://msdn.microsoft.com/en-us/library/wz42302f(v=vs.110).aspx).
		public string filePattern { get { return _filePattern; } set { _filePattern = value; UpdateDirectoryContents(); } }

		private string _filePattern = "";

		/// Called when the user hits the 'Save' button.  The passed variable
		/// is not checked for validity.
		public Callback<string> OnSave;

		/// Called if the user cancels this action.
		public Callback OnCancel;

		/**
		 * Add a callback when this window is dismissed due to 'Save' being called.
		 */
		public void AddOnSaveListener(Callback<string> listener)
		{
			if(OnSave != null)
				OnSave += listener;
			else
				OnSave = listener;
		}

		/**
		 * Add a callback when this window is canceled.
		 */
		public void AddOnCancelListener(Callback listener)
		{
			if(OnCancel != null)
				OnCancel += listener;
			else
				OnCancel = listener;
		}

		private bool mInitialized = false;

		void Awake()
		{
			if(!mInitialized)
				Initialize();
		}

		void Initialize()
		{
			mInitialized = true;

			backButton.onClick.RemoveAllListeners();
			forwardButton.onClick.RemoveAllListeners();
			upButton.onClick.RemoveAllListeners();
			cancelButton.onClick.RemoveAllListeners();
			saveButton.onClick.RemoveAllListeners();

			backButton.onClick.AddListener( Back );
			forwardButton.onClick.AddListener( Forward );
			upButton.onClick.AddListener( OpenParentDirectory );
			cancelButton.onClick.AddListener( Cancel );
			saveButton.onClick.AddListener( Save );

			UpdateNavButtonInteractibility();
		}

		/**
		 * Set the currently displayed directory.
		 */
		public void SetDirectory(string directory)
		{
			if(!mInitialized)
				Initialize();

			if( ValidDir(directory) )
			{
				forward.Clear();

				if( ValidDir(currentDirectory) )
					back.Push(currentDirectory);

				currentDirectory = directory;
			}

			UpdateDirectoryContents();
		}

		/**
		 * Update the contents in the scroll view with the available folders (and optionally files) in the `currentDirectory`.
		 */
		public void UpdateDirectoryContents()
		{
			// Debug.Log("scanning: " + currentDirectory);
			string[] children = Directory.GetDirectories(currentDirectory);

			UpdateNavButtonInteractibility();

			// hide the contents while working with them, otherwise you get flashes and artifacts
			scrollContent.SetActive(false);

			ClearScrollRect();

			int i = 0;

			if(children != null)
			{
				directoryCrumbsField.text = currentDirectory;

				for(int n = 0; n < children.Length; n++)
				{
					pb_SaveDialogButton button = GameObject.Instantiate(rowButtonPrefab);
					button.SetDelegateAndPath(SetDirectory, children[n]);
					
					pb_GUIStyleApplier style = button.GetComponent<pb_GUIStyleApplier>();
					style.style = i++ % 2 == 0 ? evenRowStyle : oddRowStyle;
					style.ApplyStyle();

					button.transform.SetParent(scrollContent.transform);
				}
			}

			if(isFileBrowser)
			{
				children = Directory.GetFiles(currentDirectory, string.IsNullOrEmpty(filePattern) ? "*" : filePattern);

				for(int n = 0; n < children.Length; n++)
				{
					pb_SaveDialogButton button = GameObject.Instantiate(rowButtonPrefab);
					button.SetDelegateAndPath(SetFile, children[n]);
					
					pb_GUIStyleApplier style = button.GetComponent<pb_GUIStyleApplier>();
					style.style = i++ % 2 == 0 ? evenRowStyle : oddRowStyle;
					style.ApplyStyle();

					button.transform.SetParent(scrollContent.transform);

				}
			}

			scrollContent.SetActive(true);
		}

		private void ClearScrollRect()
		{
			foreach(Transform t in scrollContent.transform)
				pb_ObjectUtility.Destroy(t.gameObject);
		}

		private bool ValidDir(string dir)
		{
			return dir != null && Directory.Exists(dir);
		}

		private void UpdateNavButtonInteractibility()
		{
			backButton.interactable = back.Count > 0;
			forwardButton.interactable = forward.Count > 0;
			upButton.interactable = ValidDir(currentDirectory) && Directory.GetParent(currentDirectory) != null;
		}

		public void OpenParentDirectory()
		{
			DirectoryInfo parent = Directory.GetParent(currentDirectory);

			if(parent == null)
				return;

			SetDirectory(parent.FullName);
		}

		public void SetFile(string path)
		{
			fileInputField.text = Path.GetFileName(path);
		}

		/**
		 * If OpenParentDirectory() has been called, this opens the Directory that 
		 * it came from.
		 */
		public void Back()
		{
			if(back.Count > 0)
			{
				forward.Push(currentDirectory);
				currentDirectory = back.Pop();
				UpdateDirectoryContents();
			}
		}

		public void Forward()
		{
			if(forward.Count > 0)
			{
				back.Push(currentDirectory);
				currentDirectory = forward.Pop();
				UpdateDirectoryContents();
			}
		}

		/**
		 * Cancel this dialog.  Calling script is responsible for closing the modal window in the `OnCancel`
		 * callback.
		 */
		public void Cancel()
		{
			if(OnCancel != null)
				OnCancel();

			pb_ModalWindow.Hide();
		}

		/**
		 * Exit dialog and call `OnSave` with the current file path.  Calling script is responsible for closing the modal window in the `OnCancel`
		 * callback.
		 */
		public void Save()
		{
			if(OnSave != null)
				OnSave( currentDirectory + "/" + GetFilePath() );
			else
				Debug.LogWarning("File dialog was dismissed by user but no callback is registered to perform the action!");

			pb_ModalWindow.Hide();
		}

		/**
		 * Read the current string in the file input field and make it an actual path (minus directory).
		 */
		private string GetFilePath()
		{
			string path = fileInputField.text;
			return path;
		}
	}
}
