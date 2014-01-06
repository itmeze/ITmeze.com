using ITmeze.Core.Views;
using ITmeze.Core.Views.Home;
using ITmeze.Web.Features;
using Nancy;

namespace ITmeze.Web.Modules
{
	public class FrontModule : BaseNancyModule
	{
		protected IViewProjectionFactory _viewFactory;

		public FrontModule(IViewProjectionFactory viewFactory)
		{
			_viewFactory = viewFactory;

			After.AddItemToEndOfPipeline(SetRecentBlogPosts);
			After.AddItemToEndOfPipeline(SetTagCloud);

			this.EnableCache();
		}

		private void SetTagCloud(NancyContext obj)
		{
			ViewBag.TagCould = 
				_viewFactory.Get<TagCloudBindingModel, TagCloudViewModel>(new TagCloudBindingModel() { Threshold = 2});
		}

		private void SetRecentBlogPosts(NancyContext obj)
		{
			ViewBag.Recent =
				_viewFactory.Get<RecentBlogPostSummaryBindingModel, RecentBlogPostSummaryViewModel>(
					new RecentBlogPostSummaryBindingModel());

			ViewBag.Sample = "test";
		}
	}

	
}