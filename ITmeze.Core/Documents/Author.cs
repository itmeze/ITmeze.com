using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ITmeze.Core.Documents
{
	public class Author
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		
		public string HashedPassword { get; set; }
		public string Email { get; set; }
		public string DisplayName { get; set; }

		//for compatibility with export
		public string Username { get; set; }

		public string[] Roles { get; set; }

		public void SetPassword(string newPassword)
		{
			
		}
	}
}