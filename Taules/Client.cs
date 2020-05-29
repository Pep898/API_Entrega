using NPoco;
using System;
using System.Collections.Generic;

namespace Taules
{

    [NPoco.TableName("Client")]
    [NPoco.PrimaryKey("ID", AutoIncrement = false)]
    [ExplicitColumns]
    public class Client
    {
        [Column("ID")]
        public string ID { get; set; }
        [Column("NOM")]
        public string Nom { get; set; }
        [Column("Correu")]
        public string Correu { get; set; }
        [Column("Telefon")]
        public string Telefon { get; set; }


        public override string ToString()
        {
            return
                "ID: " + ID + " | " +
                "NOM: " + Nom + " | " +
                "Correu: " + Correu + " | " +
                "Telefon: " + Telefon + " |";
        }

        public static implicit operator Client(List<Client> v)
        {
            throw new NotImplementedException();
        }
    }

}



