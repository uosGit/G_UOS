using System;
using System.Data;
//using System.Linq;
using System.Data.SqlClient;

using System.Net;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;

namespace UOS
{
    class FunClass
    {
        

        int xx;
        public static SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["connect"].ConnectionString);
        public static void DB_SQL_Connection()
        {
               if (con.State == System.Data.ConnectionState.Closed)
               {
                   con.Open(); 
                }
                else if (con.State == System.Data.ConnectionState.Broken)
                {
                    con.Close();
                    con.Open();
                }
        }
        public static void ClearControl(ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).Text = string.Empty;
                }
                else if (ctrl is DropDownList)
                {
                    ((DropDownList)ctrl).SelectedIndex = -1;
                }
                else if (ctrl is CheckBoxList)
                {
                    ((CheckBoxList)ctrl).SelectedIndex = -1;
                }
                else if (ctrl is ListBox)
                {
                    ((ListBox)ctrl).SelectedIndex = -1;
                }
                else if (ctrl is RadioButtonList)
                {
                    ((RadioButtonList)ctrl).SelectedIndex = -1;
                }
                else if (ctrl is Panel)
                {
                    ClearControl(ctrl.Controls);
                }
            }
        }
        public static byte[] ReSizeAndCompress_Img(string filename,  Stream strm)
        {
            bool valu = true;
            byte[] img = null;
            //string filename = Path.GetFileName(httpPostedFile.FileName);
            //string targetPath = Server.MapPath("Images/" + filename);
            //string filepath = Server.MapPath(targetPath);
            //String fileExtension = System.IO.Path.GetExtension(filename).ToLower();
            //String[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg" };
            //for (int i = 0; i < allowedExtensions.Length; i++)
            //{
            //    if (fileExtension == allowedExtensions[i])
            //    {
            //        valu = true;
            //        break;
            //    }
            //}
            if (valu)
            {
                //Stream strm = httpPostedFile.InputStream;
                double scaleFactor = 0.1;
                var image = System.Drawing.Image.FromStream(strm);
                //messageboxshow(targetPath .Length.ToString());
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                MemoryStream ms = new MemoryStream();
                thumbnailImg.Save(ms, image.RawFormat);
                img = ms.ToArray();
                // thumbnailImg.Save(targetPath, image.RawFormat);  // Save Pic
            }
            return img;
        }
        public static byte[] ReSizeAndCompress_Img( FileUpload upder )
        {
            bool valu = true;
            byte[] img = null;
            if (valu)
            {
                Stream strm = upder.PostedFile.InputStream;
                double scaleFactor = 0.1;
                var image = System.Drawing.Image.FromStream(strm);
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                MemoryStream ms = new MemoryStream();
                thumbnailImg.Save(ms, image.RawFormat);
                img = ms.ToArray();
            }
            return img;
        }
       public static int getID(string query)
        {
            int xx = 0;
            try
            {
                DB_SQL_Connection();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    try
                    {
                        xx = dr.GetInt32(0);
                    }
                    catch (Exception)
                    {
                        xx = 1;
                    }
                }
                if (xx == 0)
                    xx = 1;
                dr.Close();
            }
            catch (Exception ee)
            {
                 string script = "<script>alert('" + ee.Message + "');</script>";
   
            }
            return xx;
        }
       public void  SmsAddInQue(string dated, string Mob, string message, string detail,int sid,int cid)
        {
            string SQl = "insert into SmsQue(dated,MobNo,Message,Detail,status,sid,cid)values('" + dated + "','" + Mob + "',N'" + message + "','" + detail + "','Pending','"+sid+"','"+cid+"')";
            transationExaWNoQ(SQl);
        }
      
       public bool chakdatetim(object dat)
        {
            bool val = false ;
            try
            {
                DateTime.Parse(dat.ToString()).ToString("yyyy/MM/dd");
                val=true;
            }
            catch (Exception)
            {
                
                val=false;
            }
            return val;
        }
        public int getID_wrt(string query,SqlConnection cc,SqlTransaction tr)
        {
            try
            {
                xx = 0;
                SqlCommand cmd = new SqlCommand(query,cc,tr);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    try
                    {
                        xx = dr.GetInt32(0);
                    }
                    catch (Exception)
                    {
                        xx = 1;
                    }
                }
                if (xx == 0)
                    xx = 1;
                dr.Close();
            }
            catch (Exception ee)
            {
                string script = "<script>alert('" + ee.Message + "');</script>";
            }
            return xx;
        }
        public static SqlDataAdapter Datadp(string query)
        {
            DB_SQL_Connection();
            SqlCommand cmd = new SqlCommand(query, con);
            return new SqlDataAdapter(cmd);
            
        }
        public object Getfinlyear(string value)
        {
            int mon = DateTime.Parse(value).Month;
            int year = DateTime.Parse(value).Year;
            if (mon <= 6)
            {
                value=(year-1)+"-"+year;
            }
            else
                if (mon > 6)
                {
                    value = (year) + "-" + (year+1);
                }
            return value;

        }
       public static SqlTransaction tr;
      public static  void BeginTRansation()
        {
            DB_SQL_Connection();
            tr = con.BeginTransaction();
        }
      public static void CommetTransation()
      {
          if (tr != null)
          {
              tr.Commit();
              tr = null;
          }
        }
      public static void TrnRolBack()
        {
            if (tr != null)
            {
                tr.Rollback();
                tr = null;
            }
        }
        public static bool transationExaWNoQ(string query)
        {
            bool vlu = false;
            try
            {
                if (tr != null)
                {
                    if (new SqlCommand(query,con,tr).ExecuteNonQuery() > 0)

                        vlu = true;
                    else
                        vlu = false;
                }
                else
                    if (tr == null)
                    {
                        DB_SQL_Connection();
                        if (new SqlCommand(query, con, tr).ExecuteNonQuery() > 0)

                            vlu = true;
                        else
                            vlu = false;
                    }
            }
            catch (Exception ee)
            {
                string script = "<script>alert('" + ee.Message + "');</script>";
                TrnRolBack();
                vlu = false;
            }
            return vlu;
        }
        public SqlDataAdapter Datadp_wtr(string query,SqlConnection cc,SqlTransaction tr)
        {
            SqlCommand cmd = new SqlCommand(query, cc,tr);
            return new SqlDataAdapter(cmd);

        }
        public object GetNumCode(int valu,int lenth)
        {
            object code=null;
            if (valu.ToString().Length < lenth)
            {
                int relenth = lenth - valu.ToString().Length;
                for (int i = 0; i < relenth; i++)
                {
                    code += "0";
                }
                code += valu.ToString();
            }
            else
            {
                code = valu;
            }
            return code;
        }
        
        public static object singleValuQuery(string query)
        {
            SqlDataReader dr=fchDta(query);
            object data = null;
            if (dr.Read())
            {
                data = dr.GetValue(0);
            }
            dr.Close();
            return data;
        }
        public static bool convertBool(object valu)
        {
            bool vlue;
            try
            {
                vlue = Convert.ToBoolean(valu);
            }
            catch (Exception)
            {
                vlue=false;
            }
            return vlue;
        }
       
       
       

       
      
        bool valu;
        bool up, dl;
        public static int MAxID(string tb, string ID)
        {
            return getID("Select isnull(max("+ID+")+1,1) from "+tb+"");

        }
        public static DataTable dtvalue(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter adp = Datadp(query);
                adp.Fill(dt);
            }
            catch (Exception er)
            {
                string script = "<script>alert('" + er.Message + "/n" + query + "');</script>";
            }

            return dt;
        }
        
       
        //public  double GetPerstaeValue(object Tvalue, object perstage)
        //{
        //    double value = ConToDubl(Tvalue);
        //        value *= ConToDubl(perstage);
        //        value /= 100;
        //    return value;
        //}
       
        public DateTime contDTime(object value)
        {
            DateTime dtm;
            try
            {
                dtm = Convert.ToDateTime(value);
            }
            catch (Exception )
            {
                dtm=DateTime.Today;
            }
            return dtm;
        }
       

        public string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
                else if (char.IsUpper(array[i]))
                {
                    array[i] = char.ToLower(array[i]);
                }
                else if (array[i] == ' ' & array[i - 1] == ' ')
                {
                    break;
                }
            }
            return new string(array);
        }
        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        

        public static bool ChackDAtaWoMsg(string query)
        {
            bool valu = false;
            try
            {
                DB_SQL_Connection();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    valu = true;
                }
                else
                    valu = false;
                dr.Close();
            }
            catch (Exception ee)
            {
                string script = "<script>alert('" + ee.Message + "');</script>";
            }
            return valu;
        }

        public bool ChackDAtaWoMsgWTR(string query,SqlConnection cc,SqlTransaction tr)
        {
            try
            {
                valu = false;
                SqlCommand cmd = new SqlCommand(query, cc,tr);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    // MessageBox.Show("Sorry! This Name is Already Existed");
                    valu = true;
                }
                else
                    valu = false;
                dr.Close();
            }
            catch (Exception ee)
            {
                string script = "<script>alert('" + ee.Message + "');</script>";
            }
            return valu;
        }

        
        
        public static SqlDataReader fchDta(string query)
        {
            DB_SQL_Connection();
            SqlCommand cmd = new SqlCommand(query, con);
            //MessageBox.Show(cmd.CommandText);
            return cmd.ExecuteReader();
        }
       
        public SqlDataReader fchDta_wtr(string query,SqlConnection cc,SqlTransaction tr)
        {
            SqlCommand cmd = new SqlCommand(query, cc,tr);
            //MessageBox.Show(cmd.CommandText);
            return cmd.ExecuteReader();
        }
       
      
     
        public int ExNonQry(string query)
        {
            try
            {
                xx = 0;
               DB_SQL_Connection();
            SqlCommand cmd = new SqlCommand(query, con);
            // MessageBox.Show(cmd.CommandText);
            xx= cmd.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                string script = "<script>alert('" + ee.Message + "');</script>";
            }
            return xx;
        }
        //public void WaitNSeconds(int send)
        //{
        //    if (send < 1) return;
        //    DateTime _desired = DateTime.Now.AddSeconds(send);
        //    while (DateTime.Now < _desired)
        //    {
        //        System.Windows.Forms.Application.DoEvents();
        //    }
        //}
        public static object GetSngVlWQury(string query)
        {
            string value = "";
            SqlDataReader reder = fchDta(query);
            if (reder.Read())
            {
                value = reder.GetValue(0).ToString();
            }
            reder.Close();
            return value;
        }
   
        public static object GetSngVlWQury_wtr(string query,SqlConnection cc,SqlTransaction trn)
        {
            string value = "";
            SqlDataReader dr = new SqlCommand(query,cc,trn).ExecuteReader();
            if (dr.Read())
            {
                value = dr.GetValue(0).ToString();
            }
            dr.Close();
            return value;
        }
        
        
       public static double ConToDubl(object value)
        {
            double val = 0;
            try
            {
                val = Convert.ToDouble(value);
            }
            catch (Exception)
            {
                val = 0;
            }
            return val;
        }
       public Decimal ConToDisml(object value)
       {
           Decimal val = 0;
           try
           {
               val = Convert.ToDecimal(value);
           }
           catch (Exception)
           {
               val = 0;
           }
           return val;
       }

       public static int ConToInt(object value)
       {
           int val = 0;
           try
           {
               val = Convert.ToInt32(value);
           }
           catch (Exception)
           {
               val = 0;
           }
           return val;
       }

       public bool WebSendSms(string sms, string MobileNo, string usernam, string pass, string mask,string URL)
       {
           bool vlue = false;
           var username = usernam;
           var password = pass;
           var from = mask;
           var to = MobileNo;
           var message = sms;
           string urlString = string.Format(URL, username, password, from, to, message);
           var URI = new Uri(urlString);
           WebClient wc = new WebClient();
           var res = wc.DownloadString(URI);
           vlue = true;
           return vlue;
       }
      public string CheckMobNum(string num)
       {
           if (num[0] == '0')
           {
               num =  num.Remove(0,1);
           }
           if (num[3] == '-')
           {
               num = num.Remove(3, 1);
           }
           if (num.Length == 10 && num[0] == '3' )
           { num = "92" + num; }
           else
               num = "";
           return num;
       }
        public static void CmbFill(string name ,string id ,string tabe, DropDownList cmb)
      {
          string Sql = "Select distinct " + name + "," + id + " from " + tabe + " ";
          cmb.DataSource = dtvalue(Sql);
          cmb.DataValueField = id;
          cmb.DataTextField = name;
          cmb.DataBind();
         // cmb.SelectedIndex = -1;
      }
      //public static byte[] concatAndAddContent(List<byte[]> pdfByteContent)
      //{

      //    using (var ms = new MemoryStream())
      //    {
      //        using (var doc = new Document())
      //        {
      //            using (var copy = new PdfSmartCopy(doc, ms))
      //            {
      //                doc.Open();

      //                //Loop through each byte array
      //                foreach (var p in pdfByteContent)
      //                {

      //                    //Create a PdfReader bound to that byte array
      //                    using (var reader = new PdfReader(p))
      //                    {

      //                        //Add the entire document instead of page-by-page
      //                        copy.AddDocument(reader);
      //                    }
      //                }

      //                doc.Close();
      //            }
      //        }
      //        //  ms.Flush();
      //        //Return just before disposing
      //        return ms.ToArray();
      //    }
      //}
       
    }
}
