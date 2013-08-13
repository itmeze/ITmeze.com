using System;
using ITmeze.Core;
using ITmeze.Core.Commands.Posts;
using ITmeze.Core.Views;
using ITmeze.Core.Views.Admin;
using MongoDB.Driver;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using CommandResult = ITmeze.Core.Commands.CommandResult;

namespace ITmeze.Web.Modules
{
	public class AdminPostsModule : SecureModule
	{
		private readonly ICommandInvokerFactory _commandInvokerFactory;

		public AdminPostsModule(MongoDatabase database, IViewProjectionFactory factory, ICommandInvokerFactory commandInvokerFactory ) : base(database, factory)
		{
			_commandInvokerFactory = commandInvokerFactory;
			Get["/admin/posts/{page?1}"] = _ => ShowPosts(_.page);
			Get["/admin/posts/new"] = _ => ShowNewPost();
			Post["/admin/posts/new"] = _ =>
				                           {
					                           var command = this.Bind<NewPostCommand>();
					                           command.Author = CurrentUser;
					                           return CreateNewPost(command);
				                           };
			Get["/admin/posts/edit/{postId}"] = _ => ShowPostDetails(_.postId);
			Post["/admin/posts/edit/{postid}"] = _ => EditPost(this.Bind<EditPostCommand>());
		}

		private dynamic ShowNewPost()
		{
			return View["New", new NewPostCommand()];
		}

		private dynamic CreateNewPost(NewPostCommand command)
		{
			var commandResult = _commandInvokerFactory.Handle<NewPostCommand, CommandResult>(command);

			if (commandResult.Success)
			{
				AddMessage("Post was successfully added", "info");

				return this.Context.GetRedirect("/admin/posts");
			}

			AddMessage("Something went wrong while saving new post", "warn");

			return View["New", command];
		}

		private Negotiator EditPost(EditPostCommand command)
		{
			var commandResult = _commandInvokerFactory.Handle<EditPostCommand, CommandResult>(command);

			if (commandResult.Success)
			{
				AddMessage("Post was successfully updated", "info");

				return ShowPostDetails(command.PostId);
			}

			return View["Details", commandResult.GetErrors()];
		}

		private Negotiator ShowPosts(int page)
		{
			var model =
				_viewProjectionFactory.Get<AllBlogPostsBindingModel, AllBlogPostsViewModel>(new AllBlogPostsBindingModel()
				{
					Page = page,
					Take = 30
				});
			return View["Posts", model];
		}

		private Negotiator ShowPostDetails(string blogPostId)
		{
			var model =
				_viewProjectionFactory.Get<BlogPostEditBindingModel, BlogPostEditViewModel>(
					new BlogPostEditBindingModel
					{
						PostId = blogPostId
					}
				);

			return View["Details", model];
		}

	}
}