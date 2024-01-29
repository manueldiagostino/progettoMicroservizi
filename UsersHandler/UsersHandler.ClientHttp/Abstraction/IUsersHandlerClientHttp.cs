namespace UsersHandler.ClientHttp.Abstraction;

public interface IUsersHandlerClientHttp {
	public Task<HttpResponseMessage> GetUserFromId(int authorId, CancellationToken cancellationToken = default);
}
