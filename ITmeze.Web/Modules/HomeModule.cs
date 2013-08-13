using System;
using ITmeze.Core.Views;
using ITmeze.Core.Views.Home;
using Nancy.Responses.Negotiation;

namespace ITmeze.Web.Modules
{
	public class HomeModule : FrontModule
	{
		public HomeModule(IViewProjectionFactory viewFactory) : base(viewFactory)
		{
			Get["/"] = o =>
				ReturnHomeAction(new RecentBlogPostsBindingModel() { Page = 1 });
			
			Get["/page/{page?1}"] = o => 
				ReturnHomeAction(new RecentBlogPostsBindingModel(){ Page = o.page});

			Get["/tagged/{Tag}"] = parameters =>
				ReturnArticlesTaggedBy(new TaggedBlogPostsBindingModel() { Tag = parameters.tag });

			Get["/{year}/{month}/{day}/{titleslug}"] = parameters =>
				ReturnArticle(new BlogPostDetailsBindingModel { Permalink = parameters.titleslug });

			Get["/{year}/{month}/{day?}"] = parameters =>
				                                {
					                                var day = parameters["day"] == null ? null : (int?) parameters.day;
					                                var input = new IntervalBlogPostsBindingModel
						                                            {
							                                            FromDate = new DateTime(parameters.year,
							                                                                    parameters.month,
							                                                                    parameters.day ?? 1)
						                                            };

					                                input.ToDate = day.HasValue
						                                               ? input.FromDate.AddDays(1)
						                                               : input.FromDate.AddMonths(1);

					                                return ReturnArticles(input);
				                                };


		}

		public Negotiator ReturnHomeAction(RecentBlogPostsBindingModel input)
		{
			ViewBag.Title = "Recent posts";
			var model = _viewFactory.Get<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>(input);
			return View["Index", model];
		}

		public Negotiator ReturnArticle(BlogPostDetailsBindingModel input)
		{
			
			var model =
				_viewFactory.Get<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>(input);

			ViewBag.Title = model.BlogPost.Title;

			return View["details", model];
		}

		public Negotiator ReturnArticles(IntervalBlogPostsBindingModel input)
		{
			var model = _viewFactory.Get<IntervalBlogPostsBindingModel, IntervalBlogPostsViewModel>(input);

			return View["Archive", model];
		}

		public Negotiator ReturnArticlesTaggedBy(TaggedBlogPostsBindingModel input)
		{
			var model = _viewFactory.Get<TaggedBlogPostsBindingModel, TaggedBlogPostsViewModel>(input);
			return View["Tagged", model];
		}
	}
}