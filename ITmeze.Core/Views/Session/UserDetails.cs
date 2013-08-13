using System.Linq;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Session
{
	public class GetUserDetails : IViewProjection<string, Author>
	{
		private readonly MongoDatabase _database;

		public GetUserDetails(MongoDatabase database)
		{
			_database = database;
		}

		public Author Project(string input)
		{
			return _database.GetCollection<Author>("Authors").AsQueryable()
			                .FirstOrDefault(a => a.Id == input);
		}
	}
}
