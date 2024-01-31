using AuthorsHandler.ClientHttp.Abstraction;
using Microsoft.Extensions.Configuration;

namespace AuthorsHandler.ClientHttp;

public class AuthorsHandlerClientHttp : IAuthorsHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public AuthorsHandlerClientHttp(HttpClient httpClient, IConfiguration configuration) {
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(
			configuration["MusicalScoresHandlerClientHttp:AuthorsAPIBaseAddress"] 
			?? throw new FileLoadException("No such config <MusicalScoresHandlerClientHttp:AuthorsAPIBaseAddress>") 
		);
	}
	
	public async Task<HttpResponseMessage> GetAuthorFromId(int authorId, CancellationToken cancellationToken = default) {
		UriBuilder uriBuilder = new UriBuilder(_httpClient.BaseAddress + "GetAuthorFromId");
		uriBuilder.Query = $"authorId={authorId}";

		return await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
	}
}
