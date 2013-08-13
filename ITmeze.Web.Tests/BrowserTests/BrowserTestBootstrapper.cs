using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nancy;

namespace ITmeze.Web.Tests.BrowserTests
{
	public class BrowserTestBootstrapper : ITmezeBootstrapper
	{
		public override MongoDatabase Database
		{
			get
			{
				var client = new MongoClient("mongodb://localhost");

				var server = client.GetServer();

				return server.GetDatabase("ITmezeBlog_IntegrationTests");
				
			}
		}
	}

	public class RootPathProviderForTests : IRootPathProvider
	{
		public string GetRootPath()
		{
			return Environment.CurrentDirectory + @"..\..\..\..\ITmeze.Web\";
		}
	}
}
