using System.Collections.Generic;
using System.Linq;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Home
{
	public class TaggedBlogPostsViewModel
	{
		public IEnumerable<BlogPost> Posts { get; set; }
		public string Tag { get; set; }
	}

	public class TaggedBlogPostsBindingModel
	{
		public string Tag { get; set; }
	}

	public class TaggedBlogPostsViewProjection : IViewProjection<TaggedBlogPostsBindingModel, TaggedBlogPostsViewModel>
	{
		private readonly MongoDatabase _database;

		public TaggedBlogPostsViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public TaggedBlogPostsViewModel Project(TaggedBlogPostsBindingModel input)
		{
			var poset = _database.GetCollection<BlogPost>("BlogPosts")
			         .AsQueryable()
					 .Where(BlogPost.IsPublished)
					 .Where(b => b.Tags.Any(t => t.Slug == input.Tag))
					 .OrderByDescending(b => b.PubDate)
			         .Take(10)
			         .ToList();

			return new TaggedBlogPostsViewModel { Posts = poset, Tag = input.Tag };
		}
	}
}