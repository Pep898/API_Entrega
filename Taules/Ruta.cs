using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taules
{
    [NPoco.TableName("Rutes")]
    [NPoco.PrimaryKey("ID", AutoIncrement = true)]
    [ExplicitColumns]
    public class Ruta
    {
        [Column("ID")]
        public string ID { get; set; }

        [Column("Nom")]
        public string Nom { get; set; }

        [Column("Descripcio")]
        public string Descripcio { get; set; }

        [Column("Creador")]
        public int Creador { get; set; }

        [Column("Info_Ruta")]
        public string Info_Ruta { get; set; }

        [Column("Estat  ")]
        public int Estat { get; set; }

        public override string ToString()
        {
            return
                "ID: " + ID + " | " +
                "Nom: " + Nom + " | " +
                "Descripcio: " + Descripcio + " | " +
                "Creador: " + Creador + " | " +
                "Info_Ruta: " + Info_Ruta + " | " +
                "Estat: " + Estat + " | " ;
        }

        public static implicit operator Ruta(List<Usuari> v)
        {
            throw new NotImplementedException();
        }
    }

}