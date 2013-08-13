using System;
using System.Globalization;
using ITmeze.Core.Documents;
using ITmeze.Web.Infrastructure;
using Nancy.Testing;
using Xunit;

namespace ITmeze.Web.Tests.BrowserTests
{
	public class Admin : BaseTest
	{
		[Fact]
		public void registered_user_should_be_able_to_add_posts()
		{
			// Given
			
			Assert.Equal(0, _bootstrapper.Database.GetCollection<BlogPost>("BlogPosts").Count());

			var authorDocument = new Author {Email = "test@test.te", DisplayName = "tester"};
			_bootstrapper.Database.GetCollection<Author>("Authors").Insert(authorDocument);

			var authCookie = FormsAuthentication.CreateAuthCookie(authorDocument.Id);


			//when
			var browser = new Browser(_bootstrapper);

			var result = browser.Post("/admin/posts/new", with =>
				                                   {
					                                   with.Cookie(authCookie.Name, authCookie.Value);
					                                   with.HttpRequest();
													   with.FormValue("Title", "this is a test title");
													   with.FormValue("Content", "this is a test content");
													   with.FormValue("Tags", "test");
													   with.FormValue("PubDate", DateTime.Now.AddDays(-1).ToString(CultureInfo.InvariantCulture));
													   with.FormValue("Published", "true");
				                                   });

			Assert.Equal(1, _bootstrapper.Database.GetCollection<BlogPost>("BlogPosts").Count());

			var document = _bootstrapper.Database.GetCollection<BlogPost>("BlogPosts").FindOne();

			Assert.Equal(PublishStatus.Published, document.Status);
			Assert.Equal(1, document.Tags.Length);
			Assert.Equal("this is a test title", document.Title);
			Assert.Equal("tester", document.AuthorDisplayName);

		}
	}
}
