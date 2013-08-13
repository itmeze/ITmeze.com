using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Views.Home;
using ITmeze.Core.Tests;
using Xunit;

namespace ITmeze.Core.Tests.Views.Home
{
	public class TagCloudTests : MongoDatabaseBackedTest
	{
		[Fact]
		public void tag_cloud_should_return_tagt_with_tag_count()
		{
			var post1 = new BlogPost { Tags = new[] {new Tag() { Name = "test1"}, new Tag() { Name = "test2"}}}.ToPublishedBlogPost();
			var post2 = new BlogPost { Tags = new[] { new Tag() { Name = "test2" }, new Tag() { Name = "test3" } } }.ToPublishedBlogPost();
			var post3 = new BlogPost { Tags = new[] { new Tag() { Name = "test2" }, new Tag() { Name = "test3" } } }.ToPublishedBlogPost();

			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(new[] { post1, post2, post3 });

			var tagCloud = new TagCloudViewProjection(Database).Project(new TagCloudBindingModel());

			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test1").Value == 1);
			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test2").Value == 3);
			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test3").Value == 2);
		}

		[Fact]
		public void tag_cloud_should_return_tags_using_only_publishe()
		{
			var post1 = new BlogPost { Tags = new[] { new Tag() { Name = "test1" }, new Tag() { Name = "test2" } } }.ToPublishedBlogPost();
			var post2 = new BlogPost { Tags = new[] { new Tag() { Name = "test2" }, new Tag() { Name = "test3" } } }.ToPublishedBlogPost();
			var post3 = new BlogPost { Tags = new[] { new Tag() { Name = "test2" }, new Tag() { Name = "test3" } } }.ToPublishedBlogPost();

			post3.Status = PublishStatus.Draft;

			Database.GetCollection<BlogPost>("BlogPosts").InsertBatch(new[] { post1, post2, post3 });

			var tagCloud = new TagCloudViewProjection(Database).Project(new TagCloudBindingModel());

			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test1").Value == 1);
			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test2").Value == 2);
			Assert.True(tagCloud.Tags.First(t => t.Key.Name == "test3").Value == 1);
		}



	}
}
