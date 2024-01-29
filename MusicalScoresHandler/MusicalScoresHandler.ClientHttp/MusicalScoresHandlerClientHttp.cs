using Microsoft.AspNetCore.Mvc;
using MusicalScoresHandler.ClientHttp.Abstraction;

namespace MusicalScoresHandler.ClientHttp;

public class MusicalScoresHandlerClientHttp : IMusicalScoresHandlerClientHttp {
	private readonly HttpClient _httpClient;
	public MusicalScoresHandlerClientHttp(HttpClient httpClient) {
		_httpClient = httpClient;
	}
}
