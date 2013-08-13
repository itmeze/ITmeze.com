using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITmeze.Core.Document;
using ITmeze.Core.Documents;
using ITmeze.Core.Tests;
using ITmeze.Core.Views.Home;
using Xunit;

namespace ITmeze.Core.Tests.Views.Home
{
	public class TaggedBlogPostsTests : MongoDatabaseBackedTest
	{
		[Fact]
		public void tagged_returns_published_filtered_by_a_tag()
		{
			var documtnt1 = new BlogPost { Tags = new[]{ new Tag { Slug = "web", Name = "web"} }, 
				PubDate = DateTime.UtcNow.AddDays(-2), Status = PublishStatus.Published };
			var documtnt2 = new BlogPost { Tags = new[] { new Tag { Slug = "test", Name = "test" } }, 
				PubDate = DateTime.UtcNow.AddDays(-1), Status = PublishStatus.Published };

			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(new[] { documtnt1, documtnt2 });

			var taggedBlogPostsViewProjection = new TaggedBlogPostsViewProjection(Database);

			var taggedBlogPostsViewModel = taggedBlogPostsViewProjection.Project(new TaggedBlogPostsBindingModel{ Tag = "web"});

			Assert.Equal(1, taggedBlogPostsViewModel.Posts.Count());

		}
	}
}
