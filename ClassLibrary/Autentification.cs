using System;
using System.Security.Cryptography;
using System.Text;

namespace ClassLibrary
{
    public static class Autentification
    {
        public static int Register(UserRepository userRepository, string name, string login, string isModerator, string pass)
        {
            int id = 0;
            string hash = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                hash = GetHash(sha256Hash, pass);
            }
            try
            {
                id = userRepository.Insert(new User(0, name, login, isModerator, hash, null));
            }
            catch (System.Exception)
            {
                Console.WriteLine("You can't use this login. It is already taken.");
            }
            return id;
        }
        public static User Verify(UserRepository userRepository, string login, string pass)
        {
            User user = userRepository.FindLogin(login);
            if (user != null)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    if (!VerifyHash(sha256Hash, pass, user.password))
                    {
                        user = null;
                    }
                }
            }
            return user;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}