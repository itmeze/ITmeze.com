using System;
using ITmeze.Core.Views;
using ITmeze.Core.Views.Home;
using ITmeze.Web.Features;
using ITmeze.Web.Helpers;
using Nancy;

namespace ITmeze.Web.Modules
{
	public class RssModule : BaseNancyModule
	{
		private readonly IViewProjectionFactory _viewProjectionFactory;

		public RssModule(IViewProjectionFactory viewProjectionFactory)
		{
			_viewProjectionFactory = viewProjectionFactory;

			Get["/rss"] = _ => GetRecentPostsRss();
			Get["/feed"] = _ => GetRecentPostsRss();

			this.EnableCache();
		}

		private dynamic GetRecentPostsRss()
		{
			var recentPosts = _viewProjectionFactory.Get<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>(new RecentBlogPostsBindingModel()
				                                                                                  {
					                                                                                  Page = 1,
					                                                                                  Take = 30
				                                                                                  });

			return new RssResponse(recentPosts.Posts, Settings.WebsiteName, new Uri(Settings.WebsiteUrl));
		}
	}
}