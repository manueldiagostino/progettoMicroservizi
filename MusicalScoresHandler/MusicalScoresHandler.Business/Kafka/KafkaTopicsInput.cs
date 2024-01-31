using GlobalUtility.Kafka.Config;

namespace MusicalScoresHandler.Business.Kafka;

public class KafkaTopicsInput : AbstractKafkaTopics {
	public string Authors = "Authors";
	public string Users = "Users";

	public override IEnumerable<string> GetTopics() {
		return [Authors, Users];
	}
}
