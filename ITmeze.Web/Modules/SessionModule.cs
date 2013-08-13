using ITmeze.Core;
using ITmeze.Core.Commands.Member;
using ITmeze.Web.Infrastructure;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;

namespace ITmeze.Web.Modules
{
	public class SessionModule : NancyModule
	{
		private readonly ICommandInvokerFactory _commandInvoker;

		public SessionModule(ICommandInvokerFactory commandInvoker )
		{
			_commandInvoker = commandInvoker;
			
			Get["/session/login"] = _ => ShowLoginPage();
			Get["/session/logout"] = _ => Logout();
			Post["/session/login"] = _ => LoginUser(this.Bind<LoginCommand>());

		}

		public Negotiator Logout()
		{
			return View["LogoutPage"].WithCookie(FormsAuthentication.CreateLogoutCookie());
		}

		public Negotiator ShowLoginPage()
		{
			ViewBag.ReturnUrl = Request.Query.returnUrl;
			return View["LoginPage"];
		}

		public dynamic LoginUser(LoginCommand loginCommand)
		{
			var commandResult = _commandInvoker.Handle<LoginCommand, LoginCommandResult>(loginCommand);

			if (commandResult.Success)
			{
				var cookie = FormsAuthentication.CreateAuthCookie(commandResult.Author.Id);
				var response = Context.GetRedirect(loginCommand.ReturnUrl ?? "/admin");
				response.AddCookie(cookie);
				return response;
			}

			return View["LoginPage", commandResult.GetErrors()];
		}
	}
}