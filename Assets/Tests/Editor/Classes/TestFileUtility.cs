#if !UNITY_5_2

using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace GILES.Test
{
	/**
	 *	Unit tests for pb_FileUtility.
	 */
	public class TestFileUtility
	{
		const string URI_NO_EXTENSION = @"C:/Users/Test/New Unity Project/Level";
		const string URI_SPACES = @"C:/Users/Test/New Unity Project/Level.json";
		const string URI_SEPARATOR = @"C:\\Users/Test\\New%20Unity%20Project/Level.json";
		const string URI_FIXABLE = @"C:\Users/Test\\NewUnityProject/Level";

		[Test] public void MissingExtension() {
			Assert.False( pb_FileUtility.IsValidPath(URI_NO_EXTENSION, ".json") );
			Assert.False( pb_FileUtility.IsValidPath(pb_FileUtility.SanitizePath(URI_NO_EXTENSION), ".json") );
		}

		[Test] public void SpacesInPath() {
			Assert.False( pb_FileUtility.IsValidPath(URI_SPACES, ".json") );
			Assert.True( pb_FileUtility.IsValidPath(pb_FileUtility.SanitizePath(URI_SPACES), ".json") );
		}

		[Test] public void MixedSeparators() {
			Assert.False( pb_FileUtility.IsValidPath( URI_SEPARATOR, ".json") );
			Assert.True( pb_FileUtility.IsValidPath( pb_FileUtility.SanitizePath(URI_SEPARATOR), ".json") );
		}

		[Test] public void FixablePath() {
			Assert.False( pb_FileUtility.IsValidPath(URI_FIXABLE, ".json") );
			Assert.True( pb_FileUtility.IsValidPath(pb_FileUtility.SanitizePath(URI_FIXABLE, ".json"), ".json") );
		}
	}
}

#endif
