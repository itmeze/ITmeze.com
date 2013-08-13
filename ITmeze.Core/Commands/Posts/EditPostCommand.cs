using System;
using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ITmeze.Core.Commands.Posts
{
	public class EditPostCommand
	{
		public string PostId { get; set; }
		public string AuthorId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Tags { get; set; }
		public DateTime PubDate { get; set; }
		public bool Published { get; set; }
	}

	public class EditPostCommandInvoker : ICommandInvoker<EditPostCommand, CommandResult>
	{
		private readonly MongoDatabase _database;

		public EditPostCommandInvoker(MongoDatabase database)
		{
			_database = database;
		}

		public CommandResult Execute(EditPostCommand command)
		{
			var blogPostCol = _database.GetCollection<BlogPost>("BlogPosts");

			var post = blogPostCol.AsQueryable().FirstOrDefault(p => p.Id == command.PostId);

			if(post == null)
				throw new ApplicationException("Post with id: {0} was not found".FormatWith(command.PostId));

			post.Content = command.Content;
			post.PubDate = command.PubDate;
			post.Status = command.Published ? PublishStatus.Published : PublishStatus.Draft;
			post.Title = command.Title;
			post.TitleSlug = command.Title.Trim().ToSlug();
			post.Tags = command.Tags.Split(',').Select(t => new Tag {Name = t.Trim(), Slug = t.Trim().ToSlug()}).ToArray();

			blogPostCol.Save(post);

			return CommandResult.SuccessResult;
		}
	}
}
