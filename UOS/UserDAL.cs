using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS
{
    public class UserDAL
    {
        public static UserDTO ValidateUser(string Plogin , string PPassword)
        {
            if (Plogin == "Admin@mail.com" && PPassword == "Admin")
            {
                return new UserDTO
                {
                    UserID = "1",
                    Login = "Admin@mail.com",
                    Name = "Admin"
                };
                
            }
            else
            {
                return null;
            }
        }

    }

    public class UserDTO{

        public string Login { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string UserID { get; set; }
    }

}