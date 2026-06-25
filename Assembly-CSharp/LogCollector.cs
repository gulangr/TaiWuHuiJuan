using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class LogCollector
{
	// Token: 0x060000D1 RID: 209 RVA: 0x00005B00 File Offset: 0x00003D00
	public static void CollectLog(string gameRootPath = "", bool captureScreen = false, bool compression = false)
	{
		Debug.Log("尝试收集Log");
		string rootPath = Directory.GetParent(Application.dataPath).FullName;
		string outputPath = Path.Combine(rootPath, "bug");
		bool flag = Directory.Exists(outputPath);
		if (flag)
		{
			Directory.Delete(outputPath, true);
		}
		Directory.CreateDirectory(outputPath);
		string playerLogPath = Path.Combine(Application.persistentDataPath, "Player.log");
		File.Copy(playerLogPath, Path.Combine(outputPath, "Player.log"), true);
		string backEndLogOutputPath = Directory.CreateDirectory(Path.Combine(outputPath, "Logs")).FullName;
		string backEndLogPath = Path.Combine(rootPath, "Logs");
		LogCollector.CopyDirectory(backEndLogPath, backEndLogOutputPath, true, null);
		string saveDataOutputPath = Directory.CreateDirectory(Path.Combine(outputPath, "Save")).FullName;
		string saveDatePath = Path.Combine(rootPath, "Save");
		LogCollector.CopyDirectory(saveDatePath, saveDataOutputPath, true, (string s) => Path.GetExtension(s) == ".sav");
		LogCollector.CopyDirectory(saveDatePath, saveDataOutputPath, true, (string s) => Path.GetExtension(s) == ".sp");
		LogCollector.CopyDirectory(saveDatePath, saveDataOutputPath, true, (string s) => Path.GetExtension(s) == ".old");
		LogCollector.CopyDirectory(saveDatePath, saveDataOutputPath, false, null);
		if (captureScreen)
		{
			GameApp.Instance.StartCoroutine(LogCollector.ScreenCapture(Path.Combine(outputPath, "image.png")));
		}
		if (compression)
		{
			GameApp.Instance.StartCoroutine(LogCollector.Compression(outputPath));
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00005C8B File Offset: 0x00003E8B
	private static IEnumerator Compression(string path)
	{
		yield return null;
		string targetPath = path + ".zip";
		bool flag = File.Exists(targetPath);
		if (flag)
		{
			File.Delete(targetPath);
		}
		ZipFile.CreateFromDirectory(path, targetPath, CompressionLevel.Optimal, true);
		Directory.Delete(path, true);
		yield break;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00005C9A File Offset: 0x00003E9A
	private static IEnumerator ScreenCapture(string path)
	{
		yield return new WaitForEndOfFrame();
		Texture2D tex = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();
		byte[] bytes = tex.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
		yield break;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00005CAC File Offset: 0x00003EAC
	private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, Func<string, bool> filterMethod = null)
	{
		DirectoryInfo dir = new DirectoryInfo(sourceDir);
		bool flag = !dir.Exists;
		if (flag)
		{
			throw new DirectoryNotFoundException("Source directory not found: " + dir.FullName);
		}
		DirectoryInfo[] dirs = dir.GetDirectories();
		Directory.CreateDirectory(destinationDir);
		bool filter = filterMethod != null;
		foreach (FileInfo file in dir.GetFiles())
		{
			string targetFilePath = Path.Combine(destinationDir, file.Name);
			bool flag2 = filter && !filterMethod(targetFilePath);
			if (!flag2)
			{
				bool flag3 = File.Exists(targetFilePath);
				if (flag3)
				{
					File.Delete(targetFilePath);
				}
				file.CopyTo(targetFilePath, true);
			}
		}
		if (recursive)
		{
			foreach (DirectoryInfo subDir in dirs)
			{
				string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
				LogCollector.CopyDirectory(subDir.FullName, newDestinationDir, true, filterMethod);
			}
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00005DB0 File Offset: 0x00003FB0
	private static void CopyMostNewFile(string sourceDir, string destinationDir, Func<string, bool> filterMethod)
	{
		DirectoryInfo dir = new DirectoryInfo(sourceDir);
		bool flag = !dir.Exists;
		if (flag)
		{
			throw new DirectoryNotFoundException("Sorece directory not found : " + dir.FullName);
		}
		Directory.CreateDirectory(destinationDir);
		FileInfo file = (from info in dir.GetFiles()
		orderby info.CreationTime descending
		select info).FirstOrDefault((FileInfo info) => filterMethod(info.Name));
		string targetFilePath = Path.Combine(destinationDir, file.Name);
		file.CopyTo(targetFilePath, true);
	}

	// Token: 0x04000092 RID: 146
	private const string PlayerLogName = "Player.log";

	// Token: 0x04000093 RID: 147
	private const string BackEndLogName = "Logs";

	// Token: 0x04000094 RID: 148
	private const string SaveDataName = "Save";
}
