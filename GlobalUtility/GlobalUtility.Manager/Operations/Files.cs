using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace GlobalUtility.Manager.Operations;
public class Files {
	public static string? SaveFileToDir(string destDir, IFormFile target) {
		if (string.IsNullOrWhiteSpace(destDir))
			return null;

		try {

			if (!Directory.Exists(destDir)) {
				Directory.CreateDirectory(destDir);
			}
			string fileName = PasswordHasher.GetCurrentTimestampInSeconds().ToString() + Path.GetExtension(target.FileName);
			string filePath = Path.Combine(destDir, fileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create)) {
				target.CopyTo(fileStream);
			}

			Console.WriteLine($"File written in: {filePath}");
			return filePath;
		} catch (Exception ex) {
			Console.WriteLine($"An error occured while saving the file: {ex.Message}");
			return null;
		}
	}

	public static bool DeleteFileIfExists(string filePath) {
		if (string.IsNullOrWhiteSpace(filePath))
			return false;

		try {

			if (File.Exists(filePath)) {
				File.Delete(filePath);
				Console.WriteLine($"Removed file: {filePath}");
				return true;
			} else {
				Console.WriteLine($"No such file: {filePath}");
				return false;
			}
		} catch (Exception ex) {
			Console.WriteLine($"An error occured while deleting: {ex.Message}");
			return false;
		}
	}

	public static string? GetAbsolutePath(string relativePath) {
		if (string.IsNullOrWhiteSpace(relativePath))
			return relativePath;

		string applicationPath = AppDomain.CurrentDomain.BaseDirectory;

		string absolutePath = Path.Combine(applicationPath, relativePath);

		absolutePath = Path.GetFullPath(new Uri(absolutePath).LocalPath);

		return absolutePath;
	}
}
