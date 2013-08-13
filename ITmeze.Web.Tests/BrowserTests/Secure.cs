using Nancy.Testing;
using Xunit;

namespace ITmeze.Web.Tests.BrowserTests
{
	public class Secure
	{
		[Fact]
		public void admin_should_redirect_lo_login_page()
		{
			// Given
			var bootstrapper = new BrowserTestBootstrapper();

			var browser = new Browser(bootstrapper);

			var result = browser.Get("/admin", with => with.HttpRequest());

			result.ShouldHaveRedirectedTo(@"/session/login?returnUrl=/admin");
		}
	}
}
