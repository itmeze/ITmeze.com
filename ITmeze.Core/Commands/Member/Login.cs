using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Security;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Commands.Member
{
	public class LoginCommand
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}

	public class LoginCommandResult : CommandResult
	{
		public LoginCommandResult() : base()
		{
			
		}

		public LoginCommandResult(string trrorMessage) : base(trrorMessage)
		{
		}

		public Author Author { get; set; }
	}

	public class LoginCommandInvoker : ICommandInvoker<LoginCommand, LoginCommandResult>
	{
		private readonly MongoDatabase _database;

		public LoginCommandInvoker(MongoDatabase database)
		{
			_database = database;
		}

		public LoginCommandResult Execute(LoginCommand loginCommand)
		{
			var hashedPassword = Hasher.GetMd5Hash(loginCommand.Password);
			
			var author = _database.GetCollection<Author>("Authors")
			                      .AsQueryable<Author>()
			                      .FirstOrDefault(a => a.Email == loginCommand.Email && a.HashedPassword == hashedPassword);

			if (author != null)
				return new LoginCommandResult() { Author = author };
			
			return new LoginCommandResult(trrorMessage: "Invalid username or password") { };
		}
	}


}
