namespace GlobalUtility.Kafka.Model;

public class TransactionalOutbox {
        public int id { get; set; }
        public string table { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
}
