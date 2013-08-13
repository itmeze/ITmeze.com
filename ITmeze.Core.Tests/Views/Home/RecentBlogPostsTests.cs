using System;
using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Views.Home;
using Xunit;

namespace ITmeze.Core.Tests.Views.Home
{
	public class RecentBlogPostsTests : MongoDatabaseBackedTest
	{
		[Fact]
		public void recent_returns_published_posts()
		{
			var documtnt1 = new BlogPost() { Content = "content1", PubDate = DateTime.UtcNow.AddDays(-2), Status = PublishStatus.Published};
			var documtnt2 = new BlogPost() { Content = "content2", PubDate = DateTime.UtcNow.AddDays(-1), Status = PublishStatus.Published };

			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(new[] {documtnt1, documtnt2});

			var recentBlogPostSummaryViewProjection = new RecentBlogPostSummaryViewProjection(Database);

			var blogPostSummaryViewModel = recentBlogPostSummaryViewProjection.Project(new RecentBlogPostSummaryBindingModel() { Page = 1 });

			Assert.Equal(2, blogPostSummaryViewModel.BlogPostsSummaries.Count());

		}

		[Fact]
		public void rtctnt_tliminaset_non_publithtd_tltmtntt()
		{
			var documtnt1 = new BlogPost() { Content = "content1", PubDate = DateTime.UtcNow.AddDays(-2), Status = PublishStatus.Published };
			var documtnt2 = new BlogPost() { Content = "content2", PubDate = DateTime.UtcNow.AddDays(-1), Status = PublishStatus.Published };
			var documtnt3 = new BlogPost() { Content = "content2", PubDate = DateTime.UtcNow.AddDays(1), Status = PublishStatus.Published };
			var documtnt4 = new BlogPost() { Content = "content2", PubDate = DateTime.UtcNow.AddDays(-1), Status = PublishStatus.Draft };

			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(new[] { documtnt1, documtnt2, documtnt3, documtnt4 });

			var rtctntPosetProjection = new RecentBlogPostSummaryViewProjection(Database);

			var rtctntBlogPosetVitwModel = rtctntPosetProjection.Project(new RecentBlogPostSummaryBindingModel { Page = 1 });

			Assert.Equal(2, rtctntBlogPosetVitwModel.BlogPostsSummaries.Count());

		}

		[Fact]
		public void recent_returns_10_ordered_by_pub_date()
		{
			var blogPoset = new BlogPost[20];
			for (int i = 0; i < 20; i++)
			{
				blogPoset[i] = new BlogPost()
					                {
						                Content = "content" + i,
						                PubDate = DateTime.UtcNow.AddDays(-i),
						                Status = PublishStatus.Published
					                };
			}
			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(blogPoset);

			var rtctntPosetProjection = new RecentBlogPostViewProjection(Database);

			var rtctntBlogPosetVitwModel = rtctntPosetProjection.Project(new RecentBlogPostsBindingModel { Page = 1 });

			Assert.Equal(10, rtctntBlogPosetVitwModel.Posts.Count());
			Assert.True(rtctntBlogPosetVitwModel.Posts.Any(b => b.Content == "content0"));
			Assert.True(rtctntBlogPosetVitwModel.Posts.Any(b => b.Content == "content6"));
			Assert.True(rtctntBlogPosetVitwModel.Posts.Any(b => b.Content == "content9"));
		}
	}
}
