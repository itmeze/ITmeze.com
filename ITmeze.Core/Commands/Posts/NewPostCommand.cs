using System;
using System.Linq;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using MongoDB.Driver;

namespace ITmeze.Core.Commands.Posts
{
	public class NewPostCommand
	{
		public Author Author { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Tags { get; set; }
		public DateTime PubDate { get; set; }
		public bool Published { get; set; }
	}

	public class NewPostCommandInvoker : ICommandInvoker<NewPostCommand, CommandResult>
	{
		private readonly MongoDatabase _database;

		public NewPostCommandInvoker(MongoDatabase database)
		{
			_database = database;
		}

		public CommandResult Execute(NewPostCommand command)
		{
			var blogPostCol = _database.GetCollection<BlogPost>("BlogPosts");

			var post = new BlogPost
				           {
							   AuthorEmail = command.Author.Email,
							   AuthorDisplayName = command.Author.DisplayName,
					           Content = command.Content,
					           PubDate = command.PubDate,
					           Status = command.Published ? PublishStatus.Published : PublishStatus.Draft,
					           Title = command.Title,
					           TitleSlug = command.Title.Trim().ToSlug(),
							   DateUTC = DateTime.UtcNow,
					           Tags =
						           (command.Tags ?? "").Split(',')
						                  .Select(t => new Tag {Name = t.Trim(), Slug = t.Trim().ToSlug()})
						                  .ToArray(),
				           };

			var result = blogPostCol.Insert(post);

			return CommandResult.SuccessResult;
		}
	}
}
