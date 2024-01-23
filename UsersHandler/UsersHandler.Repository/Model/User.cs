using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersHandler.Repository.Model;
public class User {
	public int Id { get; set; }
	public string Username { get; set; }
	public string? Name { get; set; }
	public string? Surname { get; set; }
	public string? PropicPath { get; set; }
	public long Timestamp { get; set; }
	public int BioId { get; set; }
	public string Salt { get; set; }
	public string Hash { get; set; }

	public Bio? Bio { get; set; }
}