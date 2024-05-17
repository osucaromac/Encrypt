using BanCoppel.AesDencript;
using BanCoppel.AesEncript;
using BanCoppel.RSADencript;
using BanCoppel.Util;
using log4net;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BanCoppel.DoDecript;
using BanCoppel.EncriptEncript;

namespace BanCoppel.EncriptDecrip
{

    public class DoFileEncriptDecript
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(DoFileEncriptDecript));

        public static void Main()
        {

            string? procces = ConfigurationManager.AppSettings.Get("MesFesDic");
            DateTime now = DateTime.Now;
            string fechaHoraInicioFin = now.ToString() + " a ";

            // Banderin para encriptar o desencriptar 1 = Encripta  0 = Desencripta            
            string? banEncriptDecript = ConfigurationManager.AppSettings.Get("BanEncriptDecript") ?? "";
            try
            {
                if (banEncriptDecript.Equals("1")) {
                    DoFileEncript.ProcessEncript();
                } else if (banEncriptDecript.Equals("0")) {
                    DoFileDecript.ProcessDescrypt();
                }
                else
                {
                    List<Tuple<string, string, string, string, string, string, string>> listaDatos = new List<Tuple<string, string, string, string, string, string, string>>();
                    
                       
                    logger.Error(ConfigurationManager.AppSettings.Get("MnsjOptInvalid") ?? "");
                    string descEstatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusOptInvalid") ?? "";
                    string estatusGeneralProceso = ConfigurationManager.AppSettings.Get("MnsjOptInvalid") ?? "";
                    FileUtil.CreateLog(0, listaDatos,
                                0, "",
                                fechaHoraInicioFin + " " + (DateTime.Now.ToString()), estatusGeneralProceso,
                                descEstatusGeneralProceso);
                }
            }
            catch (Exception e)
            {              
                List<Tuple<string, string, string, string, string, string, string>> listaDatos = new List<Tuple<string, string, string, string, string, string, string>>();

                Console.WriteLine("!!!!!  Error ¡¡¡¡¡ " + e.Message);
                logger.Error("!!!!!  Error ¡¡¡¡¡ " + e.Message);
                string descEstatusGeneralProceso = e.Message;
                string estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusError") ?? "";
                FileUtil.CreateLog(0, listaDatos,
                            0, "",
                            fechaHoraInicioFin + " " + (DateTime.Now.ToString()), estatusGeneralProceso,
                            descEstatusGeneralProceso);
            }

        }




        


    }
}