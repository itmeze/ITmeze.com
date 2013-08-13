using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using ITmeze.Web.Features;
using Nancy;

namespace ITmeze.Web.Modules
{
	public class BaseNancyModule : NancyModule
	{
		public BaseNancyModule()
		{
			Before += SetEmptyMessageCollection;
			Before += SetViewBagWithSettings;

			
		}

		public dynamic Settings { get; set; }

		private Response SetViewBagWithSettings(NancyContext arg)
		{
			ViewBag.Settings  = AppConfiguration.Current;

			return null;
		}

		private Response SetEmptyMessageCollection(NancyContext arg)
		{
			ViewBag.Messages = new List<ExpandoObject>();

			return null;
		}

		public void AddMessage(string msg, string type)
		{
			dynamic obj = new ExpandoObject();
			obj.Message = msg;
			obj.MsgType = type;
			ViewBag.Messages.Value.Add(obj);
		}
	}
}