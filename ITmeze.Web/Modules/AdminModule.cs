using System.IO;
using System.Linq;
using ITmeze.Core;
using ITmeze.Core.Commands.Member;
using ITmeze.Core.Views;
using MongoDB.Driver;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;

namespace ITmeze.Web.Modules
{
	public class AdminModule : SecureModule
	{
		private readonly ICommandInvokerFactory _commandInvokerFactory;
		private readonly IRootPathProvider _rootPath;

		public AdminModule(ICommandInvokerFactory commandInvokerFactory, MongoDatabase database, IViewProjectionFactory viewProjectionFactory, IRootPathProvider rootPath) : base(database, viewProjectionFactory)
		{
			_commandInvokerFactory = commandInvokerFactory;
			_rootPath = rootPath;

			Get["/admin"] = _ => Index();
			Get["/admin/changepassword"] = _ => ChangePassword();
			Post["/admin/changepassword"] = _ => ChangePassword(this.Bind<ChangePasswordCommand>());
			Post["/admin/uploadFile"] = _ => UploadFile(Request.Files.First());
		}

		private dynamic ChangePassword()
		{
			return View["ChangePassword"];
		}

		private dynamic ChangePassword(ChangePasswordCommand command)
		{
			var commandResult = _commandInvokerFactory.Handle<ChangePasswordCommand, ITmeze.Core.Commands.CommandResult>(command);

			if (commandResult.Success)
			{
				AddMessage("Password was changed", "info");

				return this.Context.GetRedirect("/admin");
			}

			AddMessage("There was a problem saving password", "error");

			return View["ChangePassword"];
		}

		private dynamic UploadFile(HttpFile file)
		{
			string filePath = Path.Combine(_rootPath.GetRootPath(), "uploads", "images", file.Name);

			using (var fileStream = File.Create(filePath))
			{
				file.Value.CopyTo(fileStream);
			}

			return View["FileUploaded",file.Name];
		}

		private dynamic Index()
		{
			return View["Index"];
		}
	}
}