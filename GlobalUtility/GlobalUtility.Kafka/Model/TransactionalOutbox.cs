namespace GlobalUtility.Kafka.Model;

public class TransactionalOutbox {
        public long id { get; set; }
        public string table { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
}
