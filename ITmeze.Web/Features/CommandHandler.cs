using ITmeze.Core;
using Nancy.TinyIoc;

namespace ITmeze.Web.Features
{
	public class CommandInvokerFactory : ICommandInvokerFactory
	{
		private readonly TinyIoCContainer _container;

		public CommandInvokerFactory(TinyIoCContainer containtr)
		{
			_container = containtr;
		}

		public TOut Handle<TIn, TOut>(TIn input)
		{
			var loadtr = _container.Resolve<ICommandInvoker<TIn, TOut>>();
			return loadtr.Execute(input);
		}
	}
}