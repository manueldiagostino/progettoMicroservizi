using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace GlobalUtility.Manager.Operations;
public class Files {
	public static readonly string ProjectName;

	static Files() {
		ProjectName = "MSL"; // Musical Scores Library
	}

	public static string SaveFileToDir(string destDir, IFormFile target) {
		if (string.IsNullOrWhiteSpace(destDir))
			throw new Exception($"IsNullOrWhiteSpace(destDir)");

		try {

			if (!Directory.Exists(destDir)) {
				Directory.CreateDirectory(destDir);
			}
			string fileName = ProjectName + '_' + GenerateRandomString(23) + '_' + target.FileName;
			string filePath = Path.Combine(destDir, fileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create)) {
				target.CopyTo(fileStream);
			}

			Console.WriteLine($"File saved into <{filePath}>");
			return filePath;
		} catch (Exception ex) {
			throw new Exception($"An error occured while saving the file: {ex.Message}");
		}
	}

	public static bool DeleteFileIfExists(string filePath) {
		if (string.IsNullOrWhiteSpace(filePath))
			return false;

		try {

			if (File.Exists(filePath)) {
				File.Delete(filePath);
				Console.WriteLine($"Removed file <{filePath}>");
				return true;
			} else {
				Console.WriteLine($"No such file <{filePath}>");
				return false;
			}
		} catch (Exception ex) {
			Console.WriteLine($"An error occured while deleting <{ex.Message}>");
			return false;
		}
	}

	public static string GetAbsolutePath(string relativePath) {
		if (string.IsNullOrWhiteSpace(relativePath))
			throw new ArgumentException("GetAbsolutePath: string.IsNullOrWhiteSpace(relativePath)");

		string absolutePath = Path.GetFullPath(relativePath);

		// string applicationPath = AppDomain.CurrentDomain.BaseDirectory;

		// string absolutePath = Path.Combine(applicationPath, relativePath);

		// absolutePath = Path.GetFullPath(new Uri(absolutePath).LocalPath);

		return absolutePath;
	}

	public static string GenerateRandomString(int length) {
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		Random random = new Random();

		char[] randomArray = new char[length];
		for (int i = 0; i < length; i++) {
			randomArray[i] = chars[random.Next(chars.Length)];
		}

		return new string(randomArray);
	}

	public static FileStreamResult GenerateZipArchive(List<string> files, string zipFileName = "") {
		zipFileName = string.IsNullOrEmpty(zipFileName)? ProjectName + '_' + GenerateRandomString(23) + ".zip" : zipFileName;

		// Percorso temporaneo per creare il file ZIP
		string tempPath = Path.Combine(Path.GetTempPath(), zipFileName);

		// Crea il file ZIP
		using (var zipArchive = ZipFile.Open(tempPath, ZipArchiveMode.Create)) {
			foreach (string file in files) {
				// Aggiungi ogni file PDF al file ZIP
				zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
			}
		}

		FileStreamResult result = new FileStreamResult(new FileStream(tempPath, FileMode.Open), "application/zip");
		File.Delete(tempPath);

		result.FileDownloadName = zipFileName;

		return result;
	}
}
