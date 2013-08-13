using System;
using ITmeze.Core.Document;
using ITmeze.Core.Documents;
using MongoDB.Driver;

namespace ITmeze.Core.Service
{
	public interface IUserService
	{
		Author ById(Guid identifier);
	}

	public class UserService : IUserService
	{
		private readonly MongoDatabase _database;

		public UserService(MongoDatabase database)
		{
			_database = database;
		}

		public Author ById(Guid identifier)
		{
			return _database.GetCollection<Author>("Authors").FindOneById(identifier.ToObjectId());
		}
	}
}
