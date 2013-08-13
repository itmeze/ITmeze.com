using System;
using ITmeze.Core.Documents;
using ITmeze.Web.Features;
using MongoDB.Driver;
using Nancy;
using Nancy.Testing;
using Xunit;

namespace ITmeze.Web.Tests.BrowserTests
{
	public class BaseTest
	{
		protected readonly Browser _browser;
		protected readonly MongoDatabase _testDatabase;
		protected readonly BrowserTestBootstrapper _bootstrapper;

		public BaseTest()
		{
			_bootstrapper = new BrowserTestBootstrapper();

			_testDatabase = _bootstrapper.Database;

			_testDatabase.DropCollection("Authors");
			_testDatabase.DropCollection("BlogPosts");

			_browser = new Browser(_bootstrapper);

			//setting configuration
			
			AppConfiguration.Current.WebsiteUrl = "";
			AppConfiguration.Current.WebsiteName = "";
			AppConfiguration.Current.WebsiteDescription = "";
			AppConfiguration.Current.WebsiteKeywords = "";
			AppConfiguration.Current.DisqusShortName = ""; 
		}
	}

	public class IndexTest : BaseTest, IDisposable
	{
		[Fact]
		public void index_should_return_status_okay()
		{
			
			
			// Given there is a single document
			_testDatabase.GetCollection<BlogPost>("BlogPosts")
			            .Insert(new BlogPost()
				                    {
										AuthorEmail = "test@test.te",
										AuthorDisplayName = "Michal",
										Title = "sample title",
					                    Tags = new[] {new Tag() {Name = "test", Slug = "test"}}
				                    });

			// When
			var result = _browser.Get("/", with => with.HttpRequest());

			// Then
			Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		}

		public void Dispose()
		{
			_testDatabase.DropCollection("Authors");
			_testDatabase.DropCollection("BlogPosts");
		}
	}
}
