using System.Linq;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Admin
{
	public class BlogPostEditBindingModel
	{	
		public string PostId { get; set; }
	}

	public class BlogPostEditViewModel
	{
		public BlogPost BlogPost { get; set; }
	}

	public class BlogPostEditViewProjection : IViewProjection<BlogPostEditBindingModel, BlogPostEditViewModel>
	{
		private readonly MongoDatabase _database;

		public BlogPostEditViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public BlogPostEditViewModel Project(BlogPostEditBindingModel input)
		{
			var post = _database.GetCollection<BlogPost>("BlogPosts")
			                     .AsQueryable()
								 .FirstOrDefault(b => b.Id == input.PostId);

			return new BlogPostEditViewModel { BlogPost = post };
		}
	}
}
