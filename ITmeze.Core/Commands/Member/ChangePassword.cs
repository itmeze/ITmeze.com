using ITmeze.Core.Documents;
using ITmeze.Core.Security;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ITmeze.Core.Commands.Member
{
	public class ChangePasswordCommand
	{
		public string AuthorId { get; set; }
		public string NewPassword { get; set; }
		public string NewPasswordConfirm { get; set; }
		public string OldPassword { get; set; }
	}

	public class ChangePasswordCommandInvoker : ICommandInvoker<ChangePasswordCommand, CommandResult> 
	{
		private readonly MongoDatabase _database;

		public ChangePasswordCommandInvoker(MongoDatabase database)
		{
			_database = database;
		}

		public CommandResult Execute(ChangePasswordCommand command)
		{
			var result = new CommandResult();

			var author = _database.GetCollection<Author>("Authors").FindOneById(new ObjectId(command.AuthorId));

			if (Hasher.GetMd5Hash(command.OldPassword) != author.HashedPassword)
			{
				return new CommandResult("Old password does not match!");
			}

			author.HashedPassword = Hasher.GetMd5Hash(command.NewPassword);

			_database.GetCollection<Author>("Authors").Save(author);

			return result;
		}
	}
}