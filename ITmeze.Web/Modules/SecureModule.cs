using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using ITmeze.Core.Views;
using ITmeze.Web.Infra;
using ITmeze.Web.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nancy;
using Nancy.Extensions;
using Nancy.Responses;
using Nancy.Security;

namespace ITmeze.Web.Modules
{
	public class SecureModule : BaseNancyModule
	{
		protected readonly MongoDatabase Database;
		protected readonly IViewProjectionFactory _viewProjectionFactory;

		public SecureModule(MongoDatabase database, IViewProjectionFactory viewProjectionFactory)
		{
			Database = database;

			_viewProjectionFactory = viewProjectionFactory;

			Before += SetContextUserFromAuthenticationCookie;
			Before += SetCurrentUserToViewBag;
			Before += SetCurrentUserToParamsForBindingPurposes;
		}

		private Response SetCurrentUserToParamsForBindingPurposes(NancyContext ctx)
		{
			ctx.Parameters.AuthorId = ctx.CurrentUser.UserName;
			return null;
		}

		private Response SetCurrentUserToViewBag(NancyContext ctx)
		{
			var author = _viewProjectionFactory.Get<string, Author>(ctx.CurrentUser.UserName);
			ViewBag.CurrentUser = author;

			return null;
		}

		public Author CurrentUser
		{
			get { return (Author)ViewBag.CurrentUser.Value; }
		}

		private Response SetContextUserFromAuthenticationCookie(NancyContext ctx)
		{
			var username = FormsAuthentication.GetAuthUsernameFromCookie(ctx);

			if (username.IsNullOrWhitespace())
				return ctx.GetRedirect("/session/login?returnUrl=" + Request.Url.Path);

			ctx.CurrentUser = new UserIdentityWrapper(username, new string[]{});

			return null;
		}
	}
}