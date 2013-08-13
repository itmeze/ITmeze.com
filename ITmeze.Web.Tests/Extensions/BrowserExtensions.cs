using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Testing;

namespace ITmeze.Web.testt.Extensions
{
	public static class BrowserExtensions
	{
		public static void ShouldHaveRedirectedTo(this BrowserResponse response, string location, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
		{
			if (response.StatusCode != HttpStatusCode.SeeOther)
			{
				throw new AssertException("status code should be SeeOther");
			}

			if (!response.Headers["Location"].Equals(location, stringComparison))
			{
				throw new AssertException(string.Format("Location should havt bttn: {0}, but wat {1}", location, response.Headers["Location"]));
			}
		}

		public static string RedirectUrl(this BrowserResponse response)
		{
			if (response.StatusCode != HttpStatusCode.SeeOther)
				throw new AssertException("status code should be SeeOther");

			return response.Headers["Location"];
		}
	}
}
