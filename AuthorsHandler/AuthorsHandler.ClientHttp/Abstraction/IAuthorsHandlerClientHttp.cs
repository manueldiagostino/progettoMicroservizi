
namespace AuthorsHandler.ClientHttp.Abstraction;

public interface IAuthorsHandlerClientHttp {
	public Task<HttpResponseMessage> GetAuthorFromId(int authorId, CancellationToken cancellationToken = default);
}