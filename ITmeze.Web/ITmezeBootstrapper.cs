using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using ITmeze.Core;
using ITmeze.Core.Service;
using ITmeze.Core.Views;
using ITmeze.Web.Features;
using MongoDB.Driver;
using Nancy;
using Nancy.Routing;
using Nancy.TinyIoc;

namespace ITmeze.Web
{
	public class ITmezeBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);

			container.Register(typeof (IUserService), typeof (UserService));
			
			RegisterIViewProjections(container);
			RegisterICommandInvoker(container);

			container.Register(typeof(MongoDatabase), (cContainer, overloads) => Database);
		}

		public virtual MongoDatabase Database
		{
			get
			{
				var client = new MongoClient(ConfigurationManager.ConnectionStrings["Itmeze.Blog"].ConnectionString);
				var server = client.GetServer();
				return server.GetDatabase(ConfigurationManager.AppSettings["dbName"]);
			}
		}

		protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);

			pipelines.BeforeRequest += CheckIfResponseIsInCache;
			pipelines.AfterRequest += AddResponseToCache;
		}

		private static Response CheckIfResponseIsInCache(NancyContext arg)
		{
			return HttpRuntime.Cache[arg.Request.Path] as Response;
		}

		private static void AddResponseToCache(NancyContext context)
		{
			if (context.Response.StatusCode != HttpStatusCode.OK)
			{
				return;
			}

			object cacheInSec = 0;
			if (!context.Items.TryGetValue(CacheExtensions.CACHE_ITEM_KEY, out cacheInSec))
				return;

			var cachedResponse = new CachedResponse(context.Response);

			context.Response = cachedResponse;

			HttpRuntime.Cache.Add(context.Request.Path, new CachedResponse(context.Response), null, Cache.NoAbsoluteExpiration,
								  TimeSpan.FromSeconds((int)cacheInSec), CacheItemPriority.Low, null);
		}
		
		public static void RegisterICommandInvoker(TinyIoCContainer container)
		{
			var commandInvokerTypes = Assembly.GetAssembly(typeof(ICommandInvoker<,>))
											  .DefinedTypes
											  .Select(t => new
											  {
												  Type = t.AsType(),
												  Interface = t.ImplementedInterfaces.FirstOrDefault(
													  i =>
													  i.IsGenericType() &&
													  i.GetGenericTypeDefinition() == typeof(ICommandInvoker<,>))
											  })
											  .Where(t => t.Interface != null)
											  .ToArray();

			foreach (var commandInvokerType in commandInvokerTypes)
			{
				container.Register(commandInvokerType.Interface, commandInvokerType.Type);
			}

			container.Register(typeof(ICommandInvokerFactory), (cContainer, overloads) => new CommandInvokerFactory(cContainer));

		}

		public static void RegisterIViewProjections(TinyIoCContainer container)
		{
			var viewProjectionTypes = Assembly.GetAssembly(typeof (IViewProjection<,>))
			                                  .DefinedTypes
			                                  .Select(t => new
				                                               {
					                                               Type = t.AsType(),
					                                               Interface = t.ImplementedInterfaces.FirstOrDefault(
						                                               i =>
						                                               i.IsGenericType() &&
						                                               i.GetGenericTypeDefinition() == typeof (IViewProjection<,>))
				                                               })
			                                  .Where(t => t.Interface != null)
			                                  .ToArray();

			foreach (var viewProjectionType in viewProjectionTypes)
			{
				container.Register(viewProjectionType.Interface, viewProjectionType.Type);
			}

			container.Register(typeof(IViewProjectionFactory), (cContainer, overloads) => new ViewProjectionFactory(cContainer));
		}
	}
}