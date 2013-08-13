using System.Text;

namespace ITmeze.Core.Security
{
	public class Hasher
	{
		public static string GetMd5Hash(string input)
		{
			var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] bs = Encoding.UTF8.GetBytes(input);
			bs = x.ComputeHash(bs);
			var s = new StringBuilder();
			foreach (byte b in bs)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			return s.ToString();

		}
	}
}
