using System;
using System.Collections.Generic;
using ITmeze.Core.Document;
using ITmeze.Core.Service;
using Nancy;
using Nancy.Security;

namespace ITmeze.Web.Infra
{
	public class UserIdentityWrapper : IUserIdentity
	{
		private readonly string _username;
		private readonly string[] _claims;

		public UserIdentityWrapper(string username, string[] claims = null)
		{
			_username = username;
			_claims = claims;
		}

		public string UserName
		{
			get { return _username; }
		}

		public IEnumerable<string> Claims
		{
			get { return _claims; }
		}
	}
}