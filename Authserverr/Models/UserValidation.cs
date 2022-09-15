using DigitalBook.Models;

namespace Authserverr.Models
{
    public class UserValidation
    {
        private readonly DigitalBooksContext _context=new  DigitalBooksContext();

        public string userName { get; set; }
        public string password { get; set; }

        public User ValidateCredentials(string UserName , string Password)
        {
            User user1 = null;
            if(_context.Users == null)
            {
                return user1;
            }
            user1 = (from x in _context.Users where x.UserName == UserName && x.UserPassword ==EncryptionDecryption.EncodePasswordToBase64(Password)select x).SingleOrDefault();
           
           
            return user1;
        }
    }
}
