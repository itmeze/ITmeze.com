using FluentAssertions;
using ITmeze.Core.Commands.Member;
using ITmeze.Core.Document;
using ITmeze.Core.Documents;
using ITmeze.Core.Security;
using Xunit;

namespace ITmeze.Core.Tests.Member
{
	public class ChangePasswordCommandTest : MongoDatabaseBackedTest
	{
		[Fact]
		public void change_password_fails_if_old_password_does_not_match()
		{
			var author = new Author() {
					             Email = "test@wp.pl",
					             HashedPassword = Hasher.GetMd5Hash("test")
				             };
			Database.GetCollection<Author>("Authors").Save(author);

			var result = new ChangePasswordCommandInvoker(Database).Execute(new ChangePasswordCommand()
				                                                   {
					                                                   AuthorId = author.Id,
					                                                   OldPassword = "wrong one",
					                                                   NewPassword = "tester",
					                                                   NewPasswordConfirm = "tester"
				                                                   });

			Assert.False(result.Success);
		}

		[Fact]
		public void change_password_changes_user_apssword()
		{
			var author = new Author()
			{
				Email = "test@wp.pl",
				HashedPassword = Hasher.GetMd5Hash("test")
			};

			Database.GetCollection<Author>("Authors").Save(author);

			new ChangePasswordCommandInvoker(Database).Execute(new ChangePasswordCommand()
			{
				AuthorId = author.Id,
				OldPassword = "test",
				NewPassword = "tester",
				NewPasswordConfirm = "tester"
			}).Success.Should().BeTrue();

			Database.GetCollection<Author>("Authors").FindOneById(author.Id.ToObjectId()).HashedPassword.Should().BeEquivalentTo(Hasher.GetMd5Hash("tester"));

		}
	}
}
