using FluentAssertions;
using ITmeze.Core.Commands.Member;
using ITmeze.Core.Documents;
using ITmeze.Core.Security;
using Xunit;

namespace ITmeze.Core.Tests.Member
{
	public class LoginCommandTests : MongoDatabaseBackedTest
	{
		[Fact]
		public void login_should_fail_if_no_users_are_in_database()
		{
			var loginCommandInvoker = new LoginCommandInvoker(Database);

			loginCommandInvoker.Execute(new LoginCommand {
												Email = "test@wp.pl",
					                            Password = "test"
				                            }).Success.Should().BeFalse();

		}

		[Fact]
		public void login_should_end_with_success_if_there_is_a_user_in_database()
		{
			Database.GetCollection<Author>("Authors")
			        .Insert(new Author() {Email = "test@wp.pl", HashedPassword = Hasher.GetMd5Hash("test")});
			
			var loginCommandInvoker = new LoginCommandInvoker(Database);

			loginCommandInvoker.Execute(new LoginCommand {
				Email = "test@wp.pl",
				Password = "test"
			}).Success.Should().BeTrue();

		}

		[Fact]
		public void login_should_fail_if_invalid_password_provided()
		{
			var documtnt = new Author() {Email = "username", HashedPassword = Hasher.GetMd5Hash("valid password")};
			
			Database.GetCollection<Author>("Authors").Insert(documtnt);

			var loginCommandInvoker = new LoginCommandInvoker(Database);

			loginCommandInvoker.Execute(new LoginCommand()
			{
				Email = "username",
				Password = "invalid password"
			}).Success.Should().BeFalse();

		}
	}
}
