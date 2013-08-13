using System;
using System.Globalization;
using System.Linq.Expressions;
using ITmeze.Core.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ITmeze.Core.Documents
{
	public class BlogPost
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string Title { get; set; }

		public string TitleSlug { get; set; }

		public string Description { get; set; }

		public string Content { get; set; }

		public PublishStatus Status { get; set; }

		public DateTime PubDate { get; set; }

		public DateTime DateUTC { get; set; }

		public Tag[] Tags { get; set; }

		public string AuthorDisplayName { get; set; }
		public string AuthorEmail { get; set; }
		
		public static Expression<Func<BlogPost, bool>> IsPublished
		{
			get { return p => p.PubDate <= DateTime.UtcNow && p.Status == PublishStatus.Published; }
		}

		public string GetLink()
		{
			return "/{0}/{1}/".FormatWith(PubDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture), TitleSlug);
		}
	}

	public static class BlogPostExtensions
	{
		public static BlogPost ToPublishedBlogPost(this BlogPost blogPost)
		{
			blogPost.PubDate = DateTime.UtcNow.AddDays(-1);
			blogPost.Status = PublishStatus.Published;
			return blogPost;
		}


	}


}