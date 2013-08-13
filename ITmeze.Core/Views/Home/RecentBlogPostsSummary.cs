using System;
using System.Collections.Generic;
using System.Linq;
using ITmeze.Core.Document;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Home
{
	public class RecentBlogPostSummaryViewModel
	{
		public IEnumerable<BlogPostSummary> BlogPostsSummaries { get; set; }
	}

	public class BlogPostSummary
	{
		public string Title { get; set; }
		public string Link { get; set; }
	}

	public class RecentBlogPostSummaryBindingModel
	{
		public int Page { get; set; }
	}

	public class RecentBlogPostSummaryViewProjection : IViewProjection<RecentBlogPostSummaryBindingModel, RecentBlogPostSummaryViewModel>
	{
		private readonly MongoDatabase _database;

		public RecentBlogPostSummaryViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public RecentBlogPostSummaryViewModel Project(RecentBlogPostSummaryBindingModel input)
		{
			var titles = _database.GetCollection<BlogPost>("BlogPosts")
			         .AsQueryable()
					 .Where(BlogPost.IsPublished)
					 .OrderByDescending(b => b.PubDate)
			         .Select(b => new BlogPostSummary(){
						 Title = b.Title,
						 Link = b.GetLink()
					 })
					 .Take(10)
			         .ToList();

			return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles};
		}
	}
}
