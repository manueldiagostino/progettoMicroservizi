using System.Diagnostics.CodeAnalysis;
using AuthorsHandler.Repository.Model;
using AuthorsHandler.Shared;
using AutoMapper;

public sealed class AssemblyMarker {
    AssemblyMarker() { }
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class InputFileProfile : Profile {
    public InputFileProfile() {
        CreateMap<AuthorDto, Author>();
        CreateMap<Author, AuthorDto>();
		// CreateMap<AuthorReadDto, Author>();
        // CreateMap<Author, AuthorReadDto>();
    }
}