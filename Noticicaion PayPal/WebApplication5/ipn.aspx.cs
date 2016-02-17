using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Text;

namespace WebApplication5
{
    public partial class ipn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Post back elegir cualquier linea segun el uso sandbox o live
            //string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            string strLive = "https://www.paypal.com/cgi-bin/webscr";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strLive);



            //Defina los valores de la solicitud de nuevo
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;



            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;



            //Enviar la solicitud de PayPal y obtener la respuesta
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();

            //Preparando para enviar al correo electrónico
            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("correodelosprofesores@gmail.com");
            correo.To.Add("Licenciados@outlook.com.ar");


            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("correodelosprofesores@gmail.com", "qsoiqzuliwweyeog");
           
            smtp.EnableSsl = true;


            if (strResponse == "VERIFIED")
            {

            correo.Subject = "Confirmacion Paypal - Bien - " + DateTime.Now;

            string mensaje="";

            try
            {
            
                string[] descomponer = strRequest.Split('&');

                string[] Archivo = descomponer[16].Split('=');
                
                for (int i=0; i <= descomponer.Length - 1; i++  )
                {

                    string linea = descomponer[i];

                    string[] dividir = linea.Split('=');

                    mensaje = mensaje + dividir[0] + ":\t" + dividir[1] + "\n";
                    
                }

                try
                {

                    smtp.Send("correodelosprofesores@gmail.com", "Licenciados@outlook.com.ar", correo.Subject, "Paypal está OK: " + Archivo[1] + "\n" + mensaje);


                    StreamWriter web = File.CreateText(Server.MapPath("~/" + Archivo[1] + ".html"));
                    
                    web.Write(strRequest);
                    
                    web.Flush();
                    web.Close();

                }
                catch (Exception)
                {

                    return;

                }

            }
            catch(Exception)
            {

                return;          

            }



            //check the payment_status is Completed
            //check that txn_id has not been previously processed
            //check that receiver_email is your Primary PayPal email
            //check that payment_amount/payment_currency are correct
            //process payment

                    }
                    else if (strResponse == "INVALID")
                    {

                        correo.Subject = "Confirmacion Paypal - Mal - " + DateTime.Now;
                        correo.Body = "PayPal esta Erroneo";

                        try
                        {

                            smtp.Send(correo);

                        }
                        catch (Exception)
                        {

                            return;

                        }

                    }
                    else
                    {
                        correo.Subject = "Confirmacion Paypal - Dudoso - " + DateTime.Now;
                        correo.Body = "PayPal recomienda verificar manualmente este pago";

                        try
                        {

                            smtp.Send(correo);

                        }
                        catch (Exception)
                        {

                            return;

                        }
                    }

                    streamIn.Close();
                }
    }

}
        