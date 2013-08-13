using System;
using MongoDB.Driver;

namespace ITmeze.Core.Tests
{
	public class MongoDatabaseBackedTest : IDisposable
	{
		private MongoDatabase _db;
		protected MongoDatabase Database
		{
			get
			{
				if (_db == null)
				{

					var client = new MongoClient("mongodb://localhost");

					var server = client.GetServer();
					
					_db = server.GetDatabase("ITmezeBlog_Tests");
				}

				return _db;
			}
		}

		public MongoDatabaseBackedTest()
		{
			Database.DropCollection("Authors");
			Database.DropCollection("BlogPosts");
		}

		public void Dispose()
		{
			Database.DropCollection("Authors");
			Database.DropCollection("BlogPosts");
		}
	}
}
