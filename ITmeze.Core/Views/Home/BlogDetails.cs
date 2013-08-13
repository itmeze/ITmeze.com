using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITmeze.Core.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Views.Home
{
	public class BlogPostDetailsBindingModel
	{	
		public string Permalink { get; set; }
	}

	public class BlogPostDetailsViewModel
	{
		public BlogPost BlogPost { get; set; }
	}

	public class BlogPostDetailsViewProjection : IViewProjection<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>
	{
		private readonly MongoDatabase _database;

		public BlogPostDetailsViewProjection(MongoDatabase database)
		{
			_database = database;
		}

		public BlogPostDetailsViewModel Project(BlogPostDetailsBindingModel input)
		{
			var post = _database.GetCollection<BlogPost>("BlogPosts")
			                     .AsQueryable()
								 .FirstOrDefault(b => b.TitleSlug == input.Permalink);

			return new BlogPostDetailsViewModel { BlogPost = post };
		}
	}
}
