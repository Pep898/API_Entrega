using NPoco;
using System;
using System.Collections.Generic;

namespace Taules
{

    [NPoco.TableName("Usuaris")]
    [NPoco.PrimaryKey("ID", AutoIncrement = true)]
    [ExplicitColumns]
    public class Usuari
    {
        [Column("ID")]
        public string ID { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Username")]
        public string Username { get; set; }
        [Column("Password")]
        public string Password { get; set; }


        public override string ToString()
        {
            return
                "ID: " + ID + " | " +
                "Email: " + Email + " | " +
                "Username: " + Username + " | " +
                "Password: " + Password + " | ";
        }

        public static implicit operator Usuari(List<Usuari> v)
        {
            throw new NotImplementedException();
        }
    }

}



