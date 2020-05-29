using Nancy;
using NPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Taules
{
    public class PostUser
    {
        public string Email;
        public string Username;
        public string Password;
    }

    public class RutaDades
    {
        public String ID;
        public String Nom;
        public String Descripcio;
        public String CreadorUsername;
        public String InfoRuta;
        public String Estat;
    }

    public class RutaComp
    {
        public String ID;
        public String Nom;
        public String Descripcio;
        public String Creador;
        public String InfoRuta;
        public int Estat;
    }
    public class API : Nancy.NancyModule
    {
        public API() : base("/")
        {
            //Get Base.
            Get["/"] = parameters =>
            {
                return "<h1>API FLYPATH</h1><br><h2>Taules:</h2><br>1. Rutes<br>2. Usuaris<br>3. Amistats<br>" +
                "4. Estats";
            };
            //establex la ruta de al connexio a al base
            var connexio = @"Data Source=192.168.100.11;Initial Catalog=FlyPath;User ID=sa;Password=123";
            //Error 500, InternalServerError.
            Response my500 = new Response
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            //Error 41, Unauthorized.
            Response my401 = new Response
            {
                StatusCode = HttpStatusCode.Unauthorized
            };
            //Error 404, NotFound.
            Response my404 = new Response
            {
                StatusCode = HttpStatusCode.NotFound
            };
            //Error 400, BadRequest.
            Response my400 = new Response
            {
                StatusCode = HttpStatusCode.BadRequest
            };


            //GET : Per obtenir dades i en la funcionalitat de cerca.
            //POST : Per afegir dades
            //PUT : Per actualitzar dades
            //DELETE : Per eliminar dades

            //////////////////////////////////USUARIS////////////////////////////////////

            //Get tots els usuaris.
            Get["usuaris"] = parameters =>
                {
                    try
                    {
                        using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                        {
                            var myData = myCon.Fetch<Usuari>();
                            String llista = "<h1>Llista: </h1><br>";
                            foreach (var item in myData)
                            {
                                llista += item + "<br>";
                            }
                            return fastJSON.JSON.ToJSON(myData);
                        }
                    }
                    catch (Exception)
                    {
                        return my500;
                    }
                };

            //Get un usuari amb el seu codi.
            Get["usuari/{codi}"] = parameters =>
            {
                try
                {
                    string _myUser = parameters.codi;
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Usuari>(_myUser))
                        {
                            var myData = myCon.Fetch<Usuari>(NPoco.Sql.Builder.Where("id=@0", _myUser));
                            String llista = "<h1>Llista: </h1><br>";
                            foreach (var item in myData)
                            {
                                llista += item + "<br>";
                            }
                            return llista;
                        }
                        else
                        {
                            return my400;
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            //Get comprovar usuari existeix i fer el login
            Get["usuariLogin/{Username}/{Password}"] = parameters =>
            {
                try
                {
                    string myUsername = parameters.Username;
                    string myPassword = parameters.Password;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myUsername!="" && myPassword!="" && myUsername != null && myPassword != null)
                        {
                            var myData = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("Username=@0 AND Password=@1",myUsername, myPassword));
                            return fastJSON.JSON.ToJSON(myData);
                        }
                        else
                        {
                            return my400;
                        }
                    }
                }
                catch (Exception e)
                {
                    String ee = e.Message;
                    return my500;
                }
            };

            //Post, crear registre d'un usuari.
            Post["usuariRegistre"] = parameters =>
            {
                try
                {
                    string myEmail = this.Request.Form["Email"];
                    string myUsername = this.Request.Form["Username"];
                    string myPassowrd = this.Request.Form["Password"];
                    
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var allUsers = myCon.Fetch<Usuari>(NPoco.Sql.Builder.Where("Username=@0 AND Email=@1", myUsername, myEmail));
                        
                        if (!(myEmail.Contains("@") && myEmail.Contains("."))) 
                        {
                            return my401;
                        }
                        else if (allUsers.Count == 0)
                        {
                            var myNouReg = new Usuari
                            {
                                Email = myEmail,
                                Username = myUsername,
                                Password = myPassowrd
                            };
                            myCon.Insert(myNouReg);
                            return fastJSON.JSON.ToJSON(myNouReg);
                        }
                        else
                        {
                            return my404;
                        }
                    }
                }
                catch (Exception e)
                {
                    return my500;
                }
            };

            //put, actualitza les dades d'un usuari
            Put["usuariCanvis"] = parameters =>
            {
                try
                {
                    string myID = this.Request.Form["ID"];
                    string myEmail = this.Request.Form["Email"];
                    string myUsername = this.Request.Form["Username"];
                    string myPassowrd = this.Request.Form["Password"];

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        int myData1 = myCon.Fetch<Usuari>(NPoco.Sql.Builder.Where("Email=@0", myEmail)).Count;
                        int myData2 = myCon.Fetch<Usuari>(NPoco.Sql.Builder.Where("Username=@0", myUsername)).Count;

                        if (!(myEmail.Contains("@") && myEmail.Contains(".")))
                        {
                            return my401;
                        }
                        else if(myData1>1 || myData2>1)//si hi ha un ets tu si son dos algu altre ja el te
                        {
                            return my404;
                        }
                        else
                        {
                            var myNouReg = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myID));

                            myNouReg.Email = myEmail;
                            myNouReg.Username = myUsername;
                            if (myPassowrd != null)
                            {
                                myNouReg.Password = myPassowrd;
                            }

                            myCon.Update(myNouReg);

                            return fastJSON.JSON.ToJSON(myNouReg);
                        }
                    }

                }
                catch (Exception e)
                {
                    return my500;
                }
            };
            //////////////////////////////////RUTES////////////////////////////////////

            //get, obte totes les rutes amb estat public
            Get["obtenirRutesPubliques"] = parameters =>
            {
                try
                { 
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myRutes = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("Estat=@0", 2));
                        List<RutaDades> llista = new List<RutaDades>();

                        for(int i=0; i < myRutes.Count; i++)
                        {
                            var creador = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myRutes[i].Creador));
                            var estat = myCon.Single<Ruta_Estat>(NPoco.Sql.Builder.Where("Estat=@0", myRutes[i].Estat));

                            var myRDades = new RutaDades
                            {
                                ID = myRutes[i].ID,
                                Nom = myRutes[i].Nom,
                                Descripcio = myRutes[i].Descripcio,
                                CreadorUsername = creador.Username,
                                InfoRuta = myRutes[i].Info_Ruta,
                                Estat = estat.Descripcio
                            };

                            llista.Add(myRDades);
                        };

                        var llistaJSON = fastJSON.JSON.ToJSON(llista);
                        return llistaJSON;
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            //get, obte totes les rutes amb estat privat, nomes les rutes que ell a fet i les privades
            Get["obtenirRutesPrivades/{ID_Usuari}"] = parameters =>
            {
                try
                {
                    int ID_Usuari = parameters.ID_Usuari;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myRutes = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("Creador=@0", ID_Usuari));

                        List<RutaDades> llista = new List<RutaDades>();
                        for (int i = 0; i < myRutes.Count; i++)
                        {
                            var creador = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myRutes[i].Creador));
                            var estat = myCon.Single<Ruta_Estat>(NPoco.Sql.Builder.Where("Estat=@0", myRutes[i].Estat));

                            var myRDades = new RutaDades
                            {
                                ID = myRutes[i].ID,
                                Nom = myRutes[i].Nom,
                                Descripcio = myRutes[i].Descripcio,
                                CreadorUsername = creador.Username,
                                InfoRuta = myRutes[i].Info_Ruta,
                                Estat = estat.Descripcio
                            };
                            llista.Add(myRDades);
                        };

                        var llistaJSON = fastJSON.JSON.ToJSON(llista);
                        return llistaJSON;
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            //get, obte totes les rutes compartides que pot visualitzar l'usuari, nomes las rutes de usuaris que segueix
            Get["obtenirRutesCompartides/{ID_Usuari}"] = parameters =>
            {
                try
                {
                    int ID_Usuari = parameters.ID_Usuari;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Amistats>(ID_Usuari))
                        {
                            //obte les rutes en estat public
                            var rutesComp = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("Estat=@0", 3));
                            List<RutaComp> rutesCompartides = new List<RutaComp>();
                            List<RutaComp> rutesCompartidesSeguits = new List<RutaComp>();
                            for (int i = 0; i < rutesComp.Count; i++)
                            {
                                int asd = rutesComp[i].Creador;
                                var user= myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", rutesComp[i].Creador));
                                string userCreador = user.Username;
                                var myRuta = new RutaComp
                                {
                                    ID = rutesComp[i].ID,
                                    Nom = rutesComp[i].Nom,
                                    Descripcio = rutesComp[i].Descripcio,
                                    Creador = userCreador,
                                    InfoRuta = rutesComp[i].Info_Ruta,
                                    Estat = rutesComp[i].Estat
                                };
                                rutesCompartides.Add(myRuta);
                            };

                            //obte les rutes de seguits
                            var seguits = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("seguidor=@0", ID_Usuari));
                            for (int i = 0; i < rutesCompartides.Count; i++)
                            {
                                for(int y = 0; y < seguits.Count; y++)
                                {
                                    var user = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0",seguits[y].ID_Usuari));
                                    String usernameSeguit = user.Username;
                                    if (rutesCompartides[i].Creador.Equals(usernameSeguit))
                                    {
                                        rutesCompartidesSeguits.Add(rutesCompartides[i]);
                                    }
                                }
                            }

                            var llistaJSON = fastJSON.JSON.ToJSON(rutesCompartidesSeguits);
                            return llistaJSON;
                        }
                        else
                        {
                            return my400;
                        }

                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            //get obte el numero de rutes que ha compartit l'usuari
            Get["numeroRutesCompartides/{ID_Usuari}"] = parameters =>
            {
                try
                {
                    int ID_Usuari = parameters.ID_Usuari;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        //obte les rutes en estat public
                        var rutesComp = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("Creador=@0 AND Estat=@1", ID_Usuari, 3));

                        if (rutesComp.Count() < 1)
                        {
                            return "0";
                        }
                        else
                        {
                            return rutesComp.Count().ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            //get, obte l'informació d'una sola ruta
            Get["obtenirRuta/{idRuta}"] = parameters =>
            {
                try
                {
                    int idRuta = parameters.idRuta;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Ruta>(idRuta))
                        {
                            var myRuta = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("id=@0", idRuta));

                            return fastJSON.JSON.ToJSON(myRuta);
                        }
                        else
                        {
                            return my400;
                        }
                    }
                }
                catch(Exception)
                {
                    return my500;
                }
            };

            //Post, genera una nova ruta
            Post["generarRuta"] = parameters => {
                try
                {
                    string myNom = this.Request.Form["Nom"];
                    string myDescripcio = this.Request.Form["Descripcio"];
                    int myCreador = Int32.Parse(this.Request.Form["Creador"]);
                    string myInfo_Ruta = this.Request.Form["Info_Ruta"];
                    int myEstat = Int32.Parse(this.Request.Form["Estat"]);

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myNouReg = new Ruta
                        {
                            Nom = myNom,
                            Descripcio = myDescripcio,
                            Creador = myCreador,
                            Info_Ruta = myInfo_Ruta,
                            Estat = myEstat
                        };

                        myCon.Insert(myNouReg);
                        return fastJSON.JSON.ToJSON(myNouReg);
                    }
                }
                catch(Exception e)
                {
                    String a = e.Message;
                    return my500;
                }
            };
            //Put, modifica una ruta ja existent
            Put["modificarRuta"] = parameters => {
                try
                {
                    string myID = this.Request.Form["ID"];
                    string myNom = this.Request.Form["Nom"];
                    string myDescripcio = this.Request.Form["Descripcio"];
                    string myInfo_Ruta = this.Request.Form["Info_Ruta"];
                    string myEstat = this.Request.Form["Estat"];

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myNouReg = myCon.Single<Ruta>(NPoco.Sql.Builder.Where("ID=@0", myID));

                        myNouReg.Nom = myNom;
                        myNouReg.Descripcio = myDescripcio;
                        myNouReg.Info_Ruta = myInfo_Ruta;
                        myNouReg.Estat = Int32.Parse(myEstat);

                        myCon.Update(myNouReg);
                        return fastJSON.JSON.ToJSON(myNouReg);
                    }
                }
                catch (Exception e)
                {
                    String a = e.Message;
                    return my500;
                }
            };
            //Delete, elimina una ruta ja existent 
            Delete["eliminarRuta/{idRuta}"] = parameters => {
                try
                {
                    int idRuta = parameters.idRuta;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myNouReg = myCon.Single<Ruta>(NPoco.Sql.Builder.Where("ID=@0", idRuta));
                       
                        myCon.Delete(myNouReg);
                        return "4 a zero";
                    }
                }
                catch (Exception e)
                {
                    String a = e.Message;
                    return my500;
                }
            };

            ///////////////////////////AMISTATS///////////////////////////
            //get, obte els seguidors que te l'usuari
            Get["seguidors/{idUsuari}"] = parameters =>
            {
                try
                {
                    int idUsuari = parameters.idUsuari;
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Amistats>(idUsuari))
                        {
                            var myAmistats = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("ID_Usuari=@0", idUsuari));
                            
                            List<Usuari> usuaris = new List<Usuari>();
                            for(int i = 0; i < myAmistats.Count; i++)
                            {
                                usuaris.Add(myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myAmistats[i].seguidor)));
                            }

                            return fastJSON.JSON.ToJSON(usuaris);
                        }
                        else
                        {
                            return my400;
                        }
                    }
                }
                catch (Exception e)
                {
                    string a = e.Message;
                    return my500;
                }
            };
            //Get, obte els usuaris que segueix el usuari
            Get["seguits/{idUsuari}"] = parameters =>
            {
                try
                {
                    int idUsuari = parameters.idUsuari;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myAmistats = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("Seguidor=@0", idUsuari));

                        List<Usuari> usuaris = new List<Usuari>();
                        for (int i = 0; i < myAmistats.Count; i++)
                        {
                            usuaris.Add(myCon.Single<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myAmistats[i].ID_Usuari)));
                        }

                        return fastJSON.JSON.ToJSON(usuaris);
                    }
                }
                catch (Exception e)
                {
                    string a = e.Message;
                    return my500;
                }
            };
            //get, obte el numero de seguidors que te l'usuari
            Get["numeroSeguidors/{idUsuari}"] = parameters =>
            {
                try
                {
                    int idUsuari = parameters.idUsuari;
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Amistats>(idUsuari))
                        {
                            var myAmistats = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("ID_Usuari=@0", idUsuari));
                           
                            return myAmistats.Count().ToString();
                        }
                        else
                        {
                            return "0";
                        }
                    }
                }
                catch (Exception e)
                {
                    string a = e.Message;
                    return my500;
                }
            };
            //get, obte el numero de seguidors,seguits i rutes compartides per un usuari
            Get["numeroSeguidorsSeguitsRutesCompartides/{idUsuari}"] = parameters =>
            {
                try
                {

                    int idUsuari = parameters.idUsuari;
                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        int seguidors=0;
                        int seguits=0;
                        int rutesCompartides=0;
                        //seguidors usuari
                        var mySeguidors = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("ID_Usuari=@0", idUsuari));
                        if (mySeguidors.Count() <1)
                        {
                            seguidors = 0;
                        }
                        else
                        {
                            seguidors = mySeguidors.Count();
                        }
                        //usuaris seguits
                        var mySeguits = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("Seguidor=@0", idUsuari));
                        if (mySeguits.Count() < 1)
                        {
                            seguits = 0;
                        }
                        else
                        {
                            seguidors = mySeguits.Count();
                        }

                        //rutes compartides
                        var rutesComp = myCon.Fetch<Ruta>(NPoco.Sql.Builder.Where("Creador=@0 AND Estat=@1", idUsuari, 3));

                        if (rutesComp.Count() < 1)
                        {
                            rutesCompartides =0;
                        }
                        else
                        {
                            rutesCompartides = rutesComp.Count();
                        }

                        return seguidors.ToString() + "," + seguits.ToString()+","+ rutesCompartides.ToString();
                    }
                }
                catch (Exception e)
                {
                    string a = e.Message;
                    return my500;
                }
            };
            //get, obte el numero de seguits que te un usuari
            Get["numeroSeguits/{idUsuari}"] = parameters =>
            {
                try
                {
                    int idUsuari = parameters.idUsuari;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var myAmistats = myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("Seguidor=@0", idUsuari));

                        List<Usuari> usuaris = new List<Usuari>();

                        if (myAmistats.Count < 1)
                        {
                            return "0";
                        }
                        else
                        {
                            return myAmistats.Count().ToString(); ;
                        }
                    }
                }
                catch (Exception e)
                {
                    string a = e.Message;
                    return my500;
                }
            };
            //Post, afegeix un seguidor
            Post["afegirSeguidor"] = parameters => {
                try
                {
                    string myUsername = this.Request.Form["Username"];
                    string mySeguidor = this.Request.Form["Seguidor"];

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        var Usuari = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("Username=@0", myUsername));
                        string UsuariID = Usuari.ID;
                        var Seguidor = myCon.Single<Usuari>(NPoco.Sql.Builder.Where("Username=@0", mySeguidor));
                        string SeguidorID = Seguidor.ID;

                        if (myCon.Fetch<Amistats>(NPoco.Sql.Builder.Where("ID_Usuari=@0 AND Seguidor=@1", UsuariID, SeguidorID)).Count >= 1)
                        {
                            //ja segueixes aket usuari
                            return null;
                        }
                        else
                        {
                            var follower = new Amistats
                            {
                                ID_Usuari = UsuariID,
                                seguidor = SeguidorID
                            };

                            myCon.Insert(follower);
                            return fastJSON.JSON.ToJSON(follower);
                        }
                    }
                }
                catch (Exception e)
                {
                    String a = e.Message;
                    return my500;
                }
            };
            //delete, deixa de seguir a un usuari
            Delete["deixarSeguir/{Username}/{Seguidor}"] = parameters => {
                try
                {
                    int myUsername = parameters.Seguidor;
                    int myUnFollow = parameters.Username;

                    using (NPoco.Database myCon = new NPoco.Database(connexio, NPoco.DatabaseType.SqlServer2012))
                    {
                        if (myCon.Exists<Amistats>(myUnFollow))
                        {
                            var unfollow = myCon.Single<Amistats>(NPoco.Sql.Builder.Where("ID_Usuari=@0 AND Seguidor=@1", myUnFollow, myUsername));

                            //ja segueixes aket usuari
                            myCon.Delete(unfollow);
                            return "4 a zero";
                        }
                        else
                        {
                            //no segueix l'usuari aquest
                            return null;
                        }
                    }
                }
                catch (Exception e)
                {
                    String a = e.Message;
                    return my500;
                }
            };


            //////////////////////////////////OPORTUNITATS////////////////////////////////////

            /*
            //?user=Usuari3
            //Get totes les oportunitats.
            Get["oportunitat"] = parameters =>
            {
                var user = "";
                if (this.Request.Query["user"] != null)
                {
                    user = this.Request.Query["user"];
                }

                try
                {
                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        var myData = new List<Oportunitat>();
                        if (user != "")
                        {
                            myData = myCon.Fetch<Oportunitat>("WHERE Usuari_ID=@0", user);
                        }
                        else
                        {
                            myData = myCon.Fetch<Oportunitat>("WHERE acabat like 0");
                        }

                        if (myData.Count > 1)
                        {
                            return fastJSON.JSON.ToJSON(myData);
                        }
                        else
                        {
                            return my404;
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Get una oporutunitat.

            Get["oportunitat/{codi}"] = parameters =>
            {
                try
                {
                    string myOp = parameters.codi;

                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        var myData = myCon.Single<Oportunitat>(NPoco.Sql.Builder.Where("id=@0", myOp));
                        return fastJSON.JSON.ToJSON(myData);

                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Post una Oportunitat.
            Post["oportunitat/{codi}"] = parameters =>
            {
                try
                {
                    string myUserID = parameters.codi;
                    PostUser pepito = fastJSON.JSON.ToObject<PostUser>(this.Request.Form["pepito"]);

                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {

                        if (myCon.Fetch<Usuari>(NPoco.Sql.Builder.Where("ID=@0", myUserID)).Any())
                        {
                            var myData = myCon.Fetch<Oportunitat>();
                            var myNouReg = new Oportunitat();

                            myNouReg = new Oportunitat
                            {
                                Nom = pepito.Nom,
                                Usuari_ID = myUserID,
                                Data_Inici = DateTime.Today,
                            };
                            if (!myCon.Fetch<Oportunitat>(NPoco.Sql.Builder.Where("Nom=@0 and Client_ID=@1 and Estat=@2", pepito.Nom)).Any())
                            {
                                myCon.Insert(myNouReg);
                                return myNouReg;
                            }
                            else
                            {
                                return my404;
                            }
                        }
                        else
                        {
                            return my404;
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Put una Oportunitat.
            Put["oportunitat/{usuari}/{codi}"] = parameters =>
             {
                 try
                 {
                     string myID = parameters.codi;
                     string myUserID = parameters.usuari;
                     PostUser pepito = fastJSON.JSON.ToObject<PostUser>(this.Request.Form["pepito"]);

                     using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                     {
                         var myNouReg = myCon.Fetch<Oportunitat>(NPoco.Sql.Builder.Where("id=@0", myID));

                         if (pepito.Nom != "" && pepito.Nom != null && pepito.Nom != myNouReg[0].Nom)
                         {
                             myNouReg[0].Nom = pepito.Nom;
                         }

                         myCon.Update(myNouReg[0]);

                         myNouReg = myCon.Fetch<Oportunitat>(NPoco.Sql.Builder.Where("id=@0", myID));
                         var result = new Oportunitat();

                         foreach (var item in myNouReg)
                         {
                             result = item;
                         }
                         return result;
                     }
                 }
                 catch (Exception)
                 {
                     return my500;
                 }
             };

            //Delete una oportunitat
            Delete["oportunitat/{codi}"] = parameters =>
            {
                try
                {
                    string myID = parameters.codi;

                    if (Int32.TryParse(myID, out int myID2))
                    {
                        using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                        {
                            if (myCon.Exists<Oportunitat>(myID))
                            {
                                var myNouReg = myCon.Fetch<Oportunitat>(NPoco.Sql.Builder.Where("id=@0", myID));

                                myCon.Delete(myNouReg[0]);
                                return "";
                            }
                            else
                            {
                                return my404;
                            }
                        }
                    }
                    else
                    {
                        return my400;
                    }
                }
                catch (Exception)
                {
                    return my500;
                }

            };

            //////////////////////////////////ESTATS////////////////////////////////////

            //Get número d'estats.
            Get["estat/count"] = parameters =>
            {
                try
                {
                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        var myData = myCon.Fetch<Estat>();
                        return myData.Count() + "";
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Get tots els estats.
            Get["estat"] = parameters =>
            {
                try
                {
                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        var myData = myCon.Fetch<Estat>();
                        if (myData.Count > 1)
                        {
                            return fastJSON.JSON.ToJSON(myData);
                        }
                        else
                        {
                            return my404;
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Get un estat.
            Get["estat/{codi}"] = parameters =>
            {
                try
                {
                    string myEstat = parameters.codi;

                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        var myData = myCon.Fetch<Estat>(NPoco.Sql.Builder.Where("id=@0", myEstat));
                        return fastJSON.JSON.ToJSON(myData);
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };

            //Post un estat.
            Post["estat"] = parameters =>
                {
                    try
                    {
                        string myNom = this.Request.Form["Nom"];

                        using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                        {
                            if (myNom != "")
                            {
                                var myNouReg = new Estat
                                {
                                    Nom = myNom
                                };

                                myCon.Insert(myNouReg);
                                return myNouReg;
                            }
                            else
                            {
                                return my400;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return my500;
                    }
                };

            //Put un estat.
            Put["estat/{codi}"] = parameters =>
            {
                try
                {
                    string myID = parameters.codi;
                    string myNom = this.Request.Form["Nom"];

                    if (Int32.TryParse(myID, out int myID2))
                    {
                        using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                        {
                            if (myCon.Exists<Estat>(myID2))
                            {
                                var myNouReg = myCon.Fetch<Estat>(NPoco.Sql.Builder.Where("id=@0", myID2));

                                if (myNom != myNouReg[0].Nom && myNom != "")
                                {
                                    myNouReg[0].Nom = myNom;
                                    myCon.Update(myNouReg[0]);

                                    return myNouReg[0];
                                }
                                else
                                {
                                    return my400;
                                }
                            }
                            else
                            {
                                return my404;
                            }
                        }
                    }
                    else
                    {
                        return my400;
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };


            //Delete un Estat.
            Delete["estat/{codi}"] = parameters =>
            {
                try
                {
                    int myID = parameters.codi;

                    using (NPoco.Database myCon = new NPoco.Database(@"Data Source=192.168.110.79;Initial Catalog=oportunitats;User ID=sa;Password=", NPoco.DatabaseType.SqlServer2012))
                    {
                        if (!myCon.Fetch<Oportunitat>("WHERE Estat=@0", myID).Any())
                        {
                            if (myCon.Exists<Estat>(myID))
                            {
                                var myNouReg = myCon.Fetch<Estat>(NPoco.Sql.Builder.Where("id=@0", myID));

                                myCon.Delete(myNouReg[0]);
                                return "";
                            }
                            else
                            {
                                return my404;
                            }
                        }
                        else
                        {
                            return my400;
                        }
                    }
                }
                catch (Exception)
                {
                    return my500;
                }
            };
            */


        }


        public bool comprovarUserLogin(String username, String password)
        {

            return false;
        }
    }
}
