using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using ITmeze.Core.Documents;
using Nancy;

namespace ITmeze.Web.Helpers
{
	public class RssResponse : Response
	{
		private string RssTitle { get; set; }
		private Uri BaseUrl { get; set; }

		public RssResponse(IEnumerable<BlogPost> model, string rssTitle, Uri baseUrl)
		{
			RssTitle = rssTitle; ;
			BaseUrl = baseUrl;

			Contents = getXmlContent(model);
			ContentType = "application/rss+xml";
			StatusCode = HttpStatusCode.OK;
		}

		private Action<Stream> getXmlContent(IEnumerable<BlogPost> model)
		{
			var items = model.Select(post => 
				new SyndicationItem(
					title: post.Title, 
					content: post.Content, 
					itemAlternateLink: new Uri(BaseUrl + post.GetLink()), 
					id: post.Id, 
					lastUpdatedTime: post.DateUTC.ToUniversalTime())
				                                 {
					                                 PublishDate = post.PubDate, Summary = new TextSyndicationContent(post.Content, TextSyndicationContentKind.Html)
				                                 })
												 .ToList();

			var feed = new SyndicationFeed(RssTitle, RssTitle, BaseUrl, items);

			var formatter = new Rss20FeedFormatter(feed);

			return stream =>
			{
				using (var writer = XmlWriter.Create(stream))
				{
					formatter.WriteTo(writer);
				}
			};
		}
	}
}