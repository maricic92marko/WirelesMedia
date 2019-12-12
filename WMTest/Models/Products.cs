using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.Mvc;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Newtonsoft.Json;

namespace WMTest.Models
{
    public class Products 
    {
        string conString = @"Data Source=MARKO-PC\SQLEXPRESS;Initial Catalog=WMDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        //id, naziv, opis, kategorija, proizvođač, dobavljač, cena
        public int id { get; set; }
        public string naziv { get; set; }
        public string opis { get; set; }
        public string kategorija { get; set; }
        public string proizvodjac { get; set; }
        public string dobavljac { get; set; }
        public decimal cena { get; set; }
        public string filepath { get; set; }

        public List<Products> Product_JSON(string result)
        {
            var ProdList = new List<Products>();
            if (result.Length > 0)
            {
                JObject jObject = JObject.Parse(result);
                JToken jUser = jObject["product"];
                for (int i = 0; i < jUser.Count(); i++)
                {
                    var prod = new Products();
                    prod.id = (int)jUser[i]["id"];
                    prod.opis = (string)jUser[i]["opis"];
                    prod.naziv = (string)jUser[i]["naziv"];
                    prod.kategorija = (string)jUser[i]["kategorija"];
                    prod.proizvodjac = (string)jUser[i]["proizvodjac"];
                    prod.dobavljac = (string)jUser[i]["dobavljac"];
                    prod.cena = (decimal)jUser[i]["cena"];
                    ProdList.Add(prod);
                }
            }
            return ProdList;
        }

        public List<Products> LoadProduct_JSON(string path)
        {
            var ProdList = new List<Products>();
            var dir = Directory.GetCurrentDirectory();

            foreach (string file in Directory.EnumerateFiles(path, "*.*"))
            {
                string contents = File.ReadAllText(file);
                if (contents.Length > 0)
                {
                    JObject jObject = JObject.Parse(contents);
                    JToken jUser = jObject["product"];
                    for (int i = 0; i < jUser.Count(); i++)
                    {
                        var prod = new Products();
                        prod.id = (int)jUser[i]["id"];
                        prod.opis = (string)jUser[i]["opis"];
                        prod.naziv = (string)jUser[i]["naziv"];
                        prod.kategorija = (string)jUser[i]["kategorija"];
                        prod.proizvodjac = (string)jUser[i]["proizvodjac"];
                        prod.dobavljac = (string)jUser[i]["dobavljac"];
                        prod.cena = (decimal)jUser[i]["cena"];
                        prod.filepath = file;
                        ProdList.Add(prod);
                    }
               
                }
            }

            return ProdList;
        }

        public void EditProduct_JSON( FormCollection collection)
        {
            var ProdList = new List<Products>();
            var dir = Directory.GetCurrentDirectory();
                string contents = File.ReadAllText(collection["filepath"]);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(contents);
                for (int i = 0; i < jsonObj["product"].Count; i++)
                {
                 if(jsonObj["product"][i]["id"] == collection["id"])
                    {
                    jsonObj["product"][i]["naziv"] = collection["naziv"]; ;
                    jsonObj["product"][i]["opis"] = collection["opis"]; ;
                    jsonObj["product"][i]["dobavljac"] = collection["dobavljac"]; ;
                    jsonObj["product"][i]["kategorija"] = collection["kategorija"]; ;
                    jsonObj["product"][i]["proizvodjac"] = collection["proizvodjac"]; ;
                    jsonObj["product"][i]["cena"] = collection["cena"]; ;

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(collection["filepath"], output);
                    }
                }
            
        }

        public void CreateProduct_JSON(string path,FormCollection collection)
        {

            dynamic productj = new JObject();
            productj.id = 0;
            productj.naziv = collection["naziv"];
            productj.opis = collection["opis"];
            productj.dobavljac = collection["dobavljac"];
            productj.kategorija = collection["kategorija"];
            productj.proizvodjac = collection["proizvodjac"];
            productj.cena = decimal.Parse(collection["cena"].ToString());
            dynamic p = new JObject();
            var product = new JArray(productj);
            p.product = product;
            File.WriteAllText(path, p.ToString());
        }

        public void DeleteProduct_JSON(string path, int id)
        {

            var ProdList = new List<Products>();
            string contents = File.ReadAllText(path);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(contents);

            if (jsonObj["product"].Count == 1)
            {
                File.Delete(path);
            }
            else
            {


                for (int i = 0; i < jsonObj["product"].Count; i++)
                {

                    if (jsonObj["product"][i]["id"] == id)
                    {
                        var val = jsonObj["product"][i].ToString();
                        //jsonObj.Remove(jsonObj["product"][i].ToString());
                        jsonObj["product"][i].Remove();
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(path, output);
                    }
                }
            }
            }

        public void SaveJSONToDB(List<Products> List)
        {
            try
            {
                //product.ID, product.Naziv, product.Opis, product.Kategorija,
                //product.Proizvodjac, product.Dobavljac, product.Cena);

                foreach (var item in List)
                {
                    string commandText = "exec Insert_proc @Naziv,@Opis,@Kategorija,@Proizvodjac,@Dobavljac,@Cena";
                    SqlDataReader rdr = null;
                    SqlConnection con = new SqlConnection(conString);
                    SqlCommand cmd = new SqlCommand(commandText, con);
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters.Add("@Naziv", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Opis", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Kategorija", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Proizvodjac", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Dobavljac", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Cena", SqlDbType.Decimal);

                    cmd.Parameters["@ID"].Value = item.id;
                    cmd.Parameters["@Naziv"].Value = item.naziv;
                    cmd.Parameters["@Opis"].Value = item.opis;
                    cmd.Parameters["@Kategorija"].Value = item.kategorija;
                    cmd.Parameters["@Proizvodjac"].Value = item.proizvodjac;
                    cmd.Parameters["@Dobavljac"].Value = item.dobavljac;
                    cmd.Parameters["@Cena"].Value = item.cena;


                    con.Open();
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var msg = (string)rdr["msg"];
                    }
                }
               

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public List<Products> LoadFromDB(int? id)
        {
            SqlDataReader rdr = null;
            SqlConnection con = new SqlConnection(conString);

            string commandText = "Select * from product" ;
            SqlCommand cmd = new SqlCommand(commandText, con);
            if (id != null)
            {
                commandText += "WHERE ID = @ID";
                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters["@ID"].Value = id;
            }
      

            con.Open();

            var ViewModelList = new List<Products>();

            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {


                var prod = new Products();
                //var iii = ii;
                
                prod.id = int.Parse(rdr["ID"].ToString());
                prod.kategorija = (string)rdr["Kategorija"];
                prod.dobavljac = (string)rdr["Dobavljac"];
                prod.opis = (string)rdr["Opis"];
                prod.naziv = (string)rdr["Naziv"];
                prod.proizvodjac = (string)rdr["Proizvodjac"];
                prod.cena = (decimal)rdr["Cena"];
                ViewModelList.Add(prod);
            }

            con.Close();
            return ViewModelList;
        }

        public void UpdateProd(FormCollection collection)
        {
            try
            {
                //product.ID, product.Naziv, product.Opis, product.Kategorija,
                //product.Proizvodjac, product.Dobavljac, product.Cena);

                string commandText = "exec Update_proc @ID,@Naziv,@Opis,@Kategorija,@Proizvodjac,@Dobavljac,@Cena";
                SqlDataReader rdr = null;
                SqlConnection con = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, con);
                cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters.Add("@Naziv", SqlDbType.VarChar);
                cmd.Parameters.Add("@Opis", SqlDbType.VarChar);
                cmd.Parameters.Add("@Kategorija", SqlDbType.VarChar);
                cmd.Parameters.Add("@Proizvodjac", SqlDbType.VarChar);
                cmd.Parameters.Add("@Dobavljac", SqlDbType.VarChar);
                cmd.Parameters.Add("@Cena", SqlDbType.Decimal);

                cmd.Parameters["@ID"].Value = collection["id"];
                cmd.Parameters["@Naziv"].Value = collection["naziv"];
                cmd.Parameters["@Opis"].Value = collection["opis"];
                cmd.Parameters["@Kategorija"].Value = collection["kategorija"];
                cmd.Parameters["@Proizvodjac"].Value = collection["proizvodjac"];
                cmd.Parameters["@Dobavljac"].Value = collection["dobavljac"];
                cmd.Parameters["@Cena"].Value = collection["cena"];


                con.Open();
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var msg = (string)rdr["msg"];
                    }

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
        public void CreateProd(FormCollection collection)
        {

            try
            {
                //product.ID, product.Naziv, product.Opis, product.Kategorija,
                //product.Proizvodjac, product.Dobavljac, product.Cena);

                string commandText = "exec insert_proc @Naziv,@Opis,@Kategorija,@Proizvodjac,@Dobavljac,@Cena";
                SqlDataReader rdr = null;
                SqlConnection con = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, con);
                //cmd.Parameters.Add("@ID", SqlDbType.Int);
                cmd.Parameters.Add("@Naziv", SqlDbType.VarChar);
                cmd.Parameters.Add("@Opis", SqlDbType.VarChar);
                cmd.Parameters.Add("@Kategorija", SqlDbType.VarChar);
                cmd.Parameters.Add("@Proizvodjac", SqlDbType.VarChar);
                cmd.Parameters.Add("@Dobavljac", SqlDbType.VarChar);
                cmd.Parameters.Add("@Cena", SqlDbType.Decimal);

                //cmd.Parameters["@ID"].Value = collection["id"];
                cmd.Parameters["@Naziv"].Value = collection["naziv"];
                cmd.Parameters["@Opis"].Value = collection["opis"];
                cmd.Parameters["@Kategorija"].Value = collection["kategorija"];
                cmd.Parameters["@Proizvodjac"].Value = collection["proizvodjac"];
                cmd.Parameters["@Dobavljac"].Value = collection["dobavljac"];
                cmd.Parameters["@Cena"].Value = collection["cena"];


                con.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var msg = (string)rdr["msg"];
                }

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

           
        }
        public void DeleteProd(int id)
        {
            string commandText = "exec Delete_proc @ID";
            SqlDataReader rdr = null;
            SqlConnection con = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand(commandText, con);
            cmd.Parameters.Add("@ID", SqlDbType.Int);
            cmd.Parameters["@ID"].Value = id;
            con.Open();
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var msg = (string)rdr["msg"];
            }

        }
    }
}