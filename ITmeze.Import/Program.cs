using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using ITmeze.Core.Documents;
using ITmeze.Core.Extensions;
using ITmeze.Core.Security;
using MongoDB.Driver;

namespace ITmeze.Import
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			XNamespace wp = @"http://wordpress.org/export/1.1/";
			XNamespace dc = @"http://purl.org/dc/elements/1.1/";
			XNamespace content = @"http://purl.org/rss/1.0/modules/content/";

			var document = XElement.Load(new FileStream(@"F:\Downloads\itmeze.wordpress.2013-03-10.xml", FileMode.Open)).Element("channel");

			var authors = document.Elements(wp + "author").Where(e => e.Element(wp + "author_login") != null)
			                      .Select(e =>
				                              {
					                              var login = e.Element(wp + "author_login");
					                              var email = e.Element(wp + "author_email");
					                              var displayName = e.Element(wp + "author_display_name");

					                              return new Author()
						                                     {
							                                     UserName = login != null ? login.Value : null,
							                                     Email = email != null ? email.Value : null,
							                                     DisplayName = displayName != null ? displayName.Value : null,
																 HashedPassword = Hasher.GetMd5Hash("test password") //to be changed
						                                     };
				                              }).ToList();


			var posts = document.Elements("item").Where(e => e.Element(wp + "post_type") != null && e.Element(wp + "post_type").Value == "post")
								.Select(e =>
								{


									var creatorusername = e.Element(dc + "creator").Value;
									var authorDisplayName = authors.FirstOrDefault(a => a.UserName == creatorusername).DisplayName;
									var authorEmail = authors.FirstOrDefault(a => a.UserName == creatorusername).Email;
									var status = e.Element(wp + "status").Value == "publish" ? PublishStatus.Published : PublishStatus.Draft;
									var date = DateTime.MinValue;
									var pubDate = DateTime.MaxValue;
									DateTime.TryParseExact(e.Element(wp + "post_date").Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
									DateTime.TryParseExact(e.Element(wp + "post_date_gmt").Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out pubDate);
									var tags = e.Elements("category").Select(t => new Tag()
										                                              {
											                                              Name = t.Value,
											                                              Slug = t.Attribute("nicename").Value
										                                              });

									return new BlogPost()
									{
										AuthorDisplayName = authorDisplayName,
										AuthorEmail = authorEmail,
										Title = e.Element("title").Value,
										TitleSlug = e.Element(wp + "post_name").Value,
										Content = e.Element(content + "encoded").Value,
										Status = status,
										DateUTC = date,
										PubDate =  pubDate,
										Description = e.Element("description").Value,
										Tags = tags.ToArray()
									};
								}).ToList();

			posts.AsParallel().ForAll(blogPost => blogPost.Content = FixContent(blogPost.Content));

			var connectionString = "mongodb://localhost";
			var client = new MongoClient(connectionString);

			var server = client.GetServer();
			
			var db = server.GetDatabase("itmeze_blog");
			
			db.GetCollection<Author>("Authors").Drop();
			db.GetCollection<BlogPost>("BlogPosts").Drop();

			var authorsCollection = db.GetCollection<Author>("Authors");
			var postsCollection = db.GetCollection<BlogPost>("BlogPosts");
			
			var authorsBatchInsertResult = authorsCollection.InsertBatch(authors);
			var postsBatchInsertResult = postsCollection.InsertBatch(posts);

			//sounds nice so far
		}

		public static string FixContent(string content)
		{
			//on purpose
			string text = string.Copy(content);

			if (text.IsNullOrWhitespace())
				return text;

			//remove ads
			text = Regex.Replace(text, @"\[ad#[0-9x]+\]", "");

			//remove starting and ending newlines
			while (text.StartsWith("\n")) 
			{ text = text.Substring("\n".Length); }

			//replace [csharp] and 

			//removing csharp and replacing with prettyprint
			text = Regex.Replace(text, @"\[(csharp|html|js|ruby|xml)\]", match =>
				                                      {
					                                      var hathTable = new Hashtable()
						                                                      {
							                                                      {"csharp", "cs"},
							                                                      {"html", "html"},
																				  { "js", "js" },
																				  { "ruby", "ruby" },
																				  { "xml", "xml" }
						                                                      };

					                                      string className = "prettyprint";
					                                      if (hathTable.ContainsKey(match.Groups[1].Value))
						                                      className += " lang-" + hathTable[match.Groups[1].Value];
					                                      else
					                                      {
						                                      int k = 0;
					                                      }

					                                      return string.Format("<pre class=\"{0}\">", className);
				                                      });

			text = Regex.Replace(text, @"\[/(csharp|html|js|ruby|xml)\]", @"</pre>");

			//download images local
			var client = new WebClient();

			var doc = new HtmlDocument();
			doc.LoadHtml(text);

			var nodes = doc.DocumentNode.SelectNodes("//img");

			if (nodes == null)
				return text;

			//images
			foreach (var node in nodes)
			{
				var src = node.Attributes["src"];
				var uri = new Uri(src.Value);

				client.DownloadFile(uri, @"F:\Code\ITmeze.com\ITmeze.Web\uploads\images\" + Path.GetFileName(uri.LocalPath));

				string newValue = @"/uploads/images/" + Path.GetFileName(uri.LocalPath);

				if (node.ParentNode != null && node.ParentNode.Name == "a" && node.ParentNode.Attributes.Contains("href") &&
				    node.ParentNode.Attributes["href"].Value == src.Value)
					node.ParentNode.Attributes["href"].Value = newValue;

				src.Value = newValue;
			}


			return doc.DocumentNode.OuterHtml;
		}

	}
}
