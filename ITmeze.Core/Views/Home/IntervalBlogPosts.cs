using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Home
{
	public class IntervalBlogPostsViewModel
	{
		public IEnumerable<BlogPost> Posts { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
	}

	public class IntervalBlogPostsBindingModel
	{
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
	}

	public class IntervalBlogPostsViewProjection : IViewProjection<IntervalBlogPostsBindingModel, IntervalBlogPostsViewModel>
	{
		private readonly MongoDatabase _database;

		public IntervalBlogPostsViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public IntervalBlogPostsViewModel Project(IntervalBlogPostsBindingModel input)
		{
			var posts = _database.GetCollection<BlogPost>("BlogPosts")
			         .AsQueryable()
					 .Where(BlogPost.IsPublished)
					 .Where(b => b.PubDate < input.ToDate && b.PubDate > input.FromDate)
					 .OrderByDescending(b => b.PubDate)
			         .ToList();

			return new IntervalBlogPostsViewModel { Posts = posts, FromDate = input.FromDate, ToDate = input.ToDate };
		}
	}
}