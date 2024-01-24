using System.Security.Cryptography;

namespace GlobalUtility.Manager.Operations;
public class PasswordHasher {
	// Numero di iterazioni consigliato per PBKDF2
	private const int Iterations = 10000;

	// Lunghezza consigliata per il salt in byte
	private const int SaltSize = 16;
	private const int KeySize = 32;

	// Lunghezza consigliata per l'hash in byte
	private const int HashSize = 32;

	public static (string Hash, string Salt) HashPassword(string password) {
		// Random salt
		byte[] salt = GenerateSalt();

		// Evaluate hash using PBKDF2
		byte[] hash = GenerateHash(password, salt);

		// converts bytes to HEX
		string hashString = Convert.ToBase64String(salt);
		string saltString = Convert.ToBase64String(hash);

		return (hashString, saltString);
	}

	public static bool VerifyPassword(string password, string storedHash, string storedSalt) {

		byte[] hash = Convert.FromBase64String(storedHash);
		byte[] salt = Convert.FromBase64String(storedSalt);

		byte[] hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

		return CryptographicOperations.FixedTimeEquals(hash, hashInput);
	}

	public static byte[] GenerateSalt() {
		return RandomNumberGenerator.GetBytes(SaltSize);
	}

	public static byte[] GenerateHash(string password, byte[] salt) {
		return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
	}

	public static long GetCurrentTimestampInSeconds() {
		DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		TimeSpan elapsedTime = DateTime.UtcNow - epochStart;

		return (long)elapsedTime.TotalSeconds;
	}
}
