using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationForWarehouse_v2.Tools
{
    internal class CreateUser
    {
        private string name;
        private string login;
        private string password;
        private string lvl_acess;

        public string Name { get => name; set => name = value; }
        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
        public string Lvl_acess { get => lvl_acess; set => lvl_acess = value; }

        public CreateUser(string name, string login, string password, string lvl_acess)
        {
            Name = name;
            Login = login;
            Password = password;
            Lvl_acess = lvl_acess;
        }
    }
}
