using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taules
{
    [NPoco.TableName("Amistats")]
    [NPoco.PrimaryKey("ID_Usuari", AutoIncrement = false)]
    [ExplicitColumns]
    public class Amistats
    {
        [Column("ID_Usuari")]
        public string ID_Usuari { get; set; }

        [Column("Seguidor")]
        public string seguidor { get; set; }

        public override string ToString()
        {
            return
                "ID_Usuari: " + ID_Usuari + " | " +
                "Seguidor: " + seguidor + " | ";
        }

    }

}