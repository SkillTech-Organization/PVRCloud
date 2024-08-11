using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.Common
{
    public class AuthCryptoHelper
    {
        private string m_bcryptSalt;

        public AuthCryptoHelper(string p_bcryptSalt)
        {
            if (string.IsNullOrEmpty(p_bcryptSalt))
            {
                throw new ApplicationException("ERROR: Application parameter AuthBCryptSalt is not set.");
            }
            m_bcryptSalt = p_bcryptSalt;
        }

        public string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, m_bcryptSalt);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
