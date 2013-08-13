using System.Collections.Generic;
using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using ITmeze.Core.Views;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Admin
{
	public class AllBlogPostsViewModel
	{
		public IEnumerable<BlogPost> Posts { get; set; }
		public int Page { get; set; }
	}

	public class AllBlogPostsBindingModel
	{
		public AllBlogPostsBindingModel()
		{
			Page = 1;
			Take = 10;
		}

		public int Page { get; set; }
		public int Take { get; set; }
	}

	public class AllBlogPostViewProjection : IViewProjection<AllBlogPostsBindingModel, AllBlogPostsViewModel>
	{
		private readonly MongoDatabase _database;

		public AllBlogPostViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public AllBlogPostsViewModel Project(AllBlogPostsBindingModel input)
		{
			var posts = _database.GetCollection<BlogPost>("BlogPosts")
			         .AsQueryable()
					 .OrderByDescending(b => b.DateUTC)
			         .TakePage(input.Page, pageSize: input.Take)
			         .ToList();

			return new AllBlogPostsViewModel {Posts = posts, Page = input.Page};
		}
	}
}