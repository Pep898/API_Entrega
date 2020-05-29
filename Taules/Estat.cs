using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taules
{
    [NPoco.TableName("Ruta_Estat")]
    [NPoco.PrimaryKey("Estat", AutoIncrement = false)]
    [ExplicitColumns]
    public class Ruta_Estat
    {
        [Column("Estat")]
        public int Estat { get; set; }

        [Column("Descripcio")]
        public string Descripcio { get; set; }

        public override string ToString()
        {
            return
                "Estat: " + Estat + " | " +
                "Descripcio: " + Descripcio + " | ";
        }

    }

}