namespace GlobalUtility.Kafka.Constants;

// Possibili operazioni
public static class Operations {
	public const string Insert = "I";
	public const string Update = "U";
	public const string Delete = "D";

	public static string GetStringValue(Enum valueEnum) {
		return valueEnum switch {
			Enumeration.Insert => Insert,
			Enumeration.Update => Update,
			Enumeration.Delete => Delete,
			_ => throw new ArgumentOutOfRangeException(nameof(valueEnum), $"{nameof(valueEnum)} contains an invalid value '{valueEnum}'")
		};
	}

	public static bool IsValid(string value) =>
		value == Insert ||
		value == Update ||
		value == Delete;

	public enum Enumeration {
		Insert,
		Update,
		Delete
	}
}