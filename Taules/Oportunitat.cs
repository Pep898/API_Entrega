using NPoco;
using System;
using System.Collections.Generic;

namespace Taules
{

    [NPoco.TableName("Oportunitat")]
    [NPoco.PrimaryKey("ID", AutoIncrement = true)]
    [ExplicitColumns]
    public class Oportunitat
    {
        [Column("ID")]
        public int ID { get; set; }
        [Column("Nom")]
        public string Nom { get; set; }
        [Column("Descripcio")]
        public string Descripcio { get; set; }
        [Column("Usuari_ID")]
        public string Usuari_ID { get; set; }
        [Column("Client_ID")]
        public string Client_ID { get; set; }
        [Column("Estat")]
        public int Estat { get; set; }
        [Column("Data_Inici")]
        public DateTime Data_Inici { get; set; }
        [Column("Acabat")]
        public int Acabat { get; set; }

        public override string ToString()
        {
            return
                "ID: " + ID + " | " +
                "Nom: " + Nom + " | " +
                "Descripció: " + Descripcio + " | " +
                "Usuari ID: " + Usuari_ID + " | " +
                "Client_ID: " + Client_ID + " | " +
                "Estat: " + Estat + " | " +
                "Data_Inici: " + Data_Inici + " | " +
                "Acabat: " + Acabat + " |";
        }

        public static implicit operator Oportunitat(List<Oportunitat> v)
        {
            throw new NotImplementedException();
        }
    }

}



