using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Home
{
	public class RecentBlogPostsViewModel
	{
		public IEnumerable<BlogPost> Posts { get; set; }
		public int Page { get; set; }
	}

	public class RecentBlogPostsBindingModel
	{
		public RecentBlogPostsBindingModel()
		{
			Page = 1;
			Take = 10;
		}

		public int Page { get; set; }
		public int Take { get; set; }
	}

	public class RecentBlogPostViewProjection : IViewProjection<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>
	{
		private readonly MongoDatabase _database;

		public RecentBlogPostViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public RecentBlogPostsViewModel Project(RecentBlogPostsBindingModel input)
		{
			var posts = _database.GetCollection<BlogPost>("BlogPosts")
			         .AsQueryable().Where(BlogPost.IsPublished)
					 .OrderByDescending(b => b.PubDate)
			         .TakePage(input.Page, pageSize: input.Take)
			         .ToList();

			return new RecentBlogPostsViewModel {Posts = posts, Page = input.Page };
		}
	}
}