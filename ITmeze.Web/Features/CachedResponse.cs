using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;

namespace ITmeze.Web.Features
{
	public class CachedResponse : Response
	{
		private readonly string _oldResponseOutput;

		public CachedResponse(Response response)
		{
			this.ContentType = response.ContentType;
			this.Headers = response.Headers;
			this.StatusCode = response.StatusCode;
			
			using (var memoryStream = new MemoryStream())
			{
				response.Contents(memoryStream);
				this._oldResponseOutput = Encoding.UTF8.GetString(memoryStream.GetBuffer().Where(a => a != 0).ToArray());
			}

			this.Contents = GetContents(this._oldResponseOutput);
		}

		protected static Action<Stream> GetContents(string contents)
		{
			return stream =>
			{
				var writer = new StreamWriter(stream) { AutoFlush = true };
				writer.Write(contents);
			};
		}
	}


}