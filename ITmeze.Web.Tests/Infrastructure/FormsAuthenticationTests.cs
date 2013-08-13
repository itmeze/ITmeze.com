using ITmeze.Web.Infrastructure;
using Xunit;

namespace ITmeze.Web.Tests.Infrastructure
{
	public class FormAuthenticationTests
	{
		[Fact]
		public void encrypted_cookie_can_be_decrypted()
		{
			var userName = "to be encrypted";

			var encrypted = FormsAuthentication.EncryptAndSignCookie(userName);

			Assert.NotEqual(encrypted, userName);

			var decrypted = FormsAuthentication.DecryptAndValidateAuthenticationCookie(encrypted);

			Assert.Equal(userName, decrypted);
		}
	}
}
