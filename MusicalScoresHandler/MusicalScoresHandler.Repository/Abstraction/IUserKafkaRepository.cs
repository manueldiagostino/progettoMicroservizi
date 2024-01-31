using MusicalScoresHandler.Repository.Model;

namespace MusicalScoresHandler.Repository.Abstraction;

public interface IUserKafkaRepository {
	public Task SaveChangesAsync(CancellationToken cancellationToken = default);
	public Task InsertUser(UserKafka userKafka, CancellationToken cancellationToken = default);
	public Task<UserKafka> UpdateUser(UserKafka userKafka, CancellationToken cancellationToken = default);
	public Task<UserKafka> DeleteUser(int userId, CancellationToken cancellationToken = default);

	public Task<bool> CheckUserId(int userId, CancellationToken cancellationToken = default);
}
