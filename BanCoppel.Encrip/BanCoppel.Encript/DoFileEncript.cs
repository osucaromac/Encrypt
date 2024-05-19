using BanCoppel.AesEncript;
using BanCoppel.Util;
using log4net;
using log4net.Util;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BanCoppel.Encript
{

    public class DoFileEncript
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DoFileEncript));
        public static void ProcessEncript()
        {
            string dia = "";
            string mes = "";
            string? mnsjExitoso = "";
            string? estatus = "";
            string? descEstatus = "";

            string? mesFesDic = ConfigurationManager.AppSettings.Get("MesFesDic");
            string? diaFesDic = ConfigurationManager.AppSettings.Get("DiaFesDic");
            string? mesFesEne = ConfigurationManager.AppSettings.Get("MesFesEne");
            string? diaFesEne = ConfigurationManager.AppSettings.Get("DiaFesEne");
            string? extArchivos = ConfigurationManager.AppSettings.Get("ExtencionArchivos");
            string? nomLogEncriptado = ConfigurationManager.AppSettings.Get("nomLogEncriptado") ?? "";
            //string? ruta_destino = ConfigurationManager.AppSettings.Get("RutaArchivosEncriptWind");
            string? ruta_destino = ConfigurationManager.AppSettings.Get("RutaArchivosEncript") ?? "";


            List<Tuple<string, string, string, string, string, string, string>> listaDatos = new List<Tuple<string, string, string, string, string, string, string>>();
            int numArchivosProcesadosFallidos = 0;
            string nomArchivosProcesadosFallidos = "";
            string fechaHoraInicioFin = "";
            int numArchivosProcesar = 0;
            string fechaInicioProceso = "";
            byte[]? cipherBytes = null;

            string? estatusGeneralProceso = "";
            string? descEstatusGeneralProceso = "";
            Aes aes = Aes.Create();

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] aesKey;
            byte[] ivParams;

            RSACryptoServiceProvider RSApublicKey;

            try
            {
                string[]? archivos_encriptar = null;
                //string? ruta_origen = ConfigurationManager.AppSettings.Get("RutaArchivosDescript");
                //string? ruta_destino = ConfigurationManager.AppSettings.Get("RutaArchivosEncript");

                DateTime now = DateTime.Now;
                dia = now.ToString().Substring(0, 2);
                mes = now.ToString().Substring(3, 2);
                try
                {
                    //string? ruta_origen = ConfigurationManager.AppSettings.Get("RutaArchivosDescriptWin") ?? "";
                    string? ruta_origen = ConfigurationManager.AppSettings.Get("RutaArchivosDescript") ?? "";

                    
                    // string? publicKey = ConfigurationManager.AppSettings.Get("RutaPublicKeyWind") ?? "";
                    string? publicKey = ConfigurationManager.AppSettings.Get("RutaPublicKey") ?? "";

                    Console.WriteLine("---------  dia:::::   " + dia);
                    Console.WriteLine("---------  mes:::::   " + mes);
                    fechaHoraInicioFin = now.ToString();
                    if (dia.Equals(diaFesDic) && mes.Equals(mesFesDic) || dia.Equals(diaFesEne) && mes.Equals(mesFesEne))
                    {
                        descEstatusGeneralProceso = ConfigurationManager.AppSettings.Get("MnsjDiaDeshabilitado") ?? "";
                        estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusDiaDeshabilitado") ?? "";
                        Console.WriteLine("---------  Execución de job deshabilitada para 25-DIC y para 01-ENE");
                    }
                    else
                    {
                        archivos_encriptar = Directory.GetFiles(ruta_origen, extArchivos ?? "");
                        numArchivosProcesar = archivos_encriptar.Length;
                        Console.WriteLine("---------  Archivos a procesar  :::::  " + archivos_encriptar.Length);
                        logger.Info("---------  Archivos a procesar  :::::  " + archivos_encriptar.Length);

                        DirectoryInfo di = new DirectoryInfo(@ruta_origen);
                        if (archivos_encriptar.Length > 0)
                        {
                            Console.WriteLine("---------  Iniciando proceso num  archivos a procesar :::::  " + archivos_encriptar.Length);
                            logger.Info("---------  Iniciando proceso encriptacion ----------------");

                            foreach (string archivo in archivos_encriptar)
                            {
                                //Generacion de llaver 
                                aes.GenerateKey();
                                aes.GenerateIV();
                                aesKey = aes.Key;
                                ivParams = aes.IV;

                                Console.WriteLine(archivo);
                                string textPlain = File.ReadAllText(archivo);
                                fechaInicioProceso = DateTime.Now.ToString() ?? "";
                                StreamReader archivos = new StreamReader(archivo);
                                var plainTextData = archivos.ReadToEnd();

                                Console.WriteLine(Convert.ToBase64String(aesKey));
                                Console.WriteLine(Convert.ToBase64String(ivParams));
                                // 2. Encrypt file content
                                byte[] encryptedContent = AESEncrip.EncryptFile(aesKey, ivParams, Encoding.UTF8.GetBytes(plainTextData));
                                RSApublicKey = RSAEncrip.ImportPublicKey(File.ReadAllText(publicKey));
                                cipherBytes = RSApublicKey.Encrypt(aesKey, false);
                                byte[] fileOutputContent = FileUtil.combineBytes(cipherBytes, encryptedContent);
                                Console.WriteLine("Texto encriptado:::::  " + cipherBytes);
                                string cipherText = Convert.ToBase64String(cipherBytes);
                                //Guardar en una ruta especifica los archivos cifrados.
                                string archivoEncriptado = Path.Combine(ruta_destino, Path.GetFileName(archivo));
                                mnsjExitoso = ConfigurationManager.AppSettings.Get("MnsjProcesaArchivoCorrecto") ?? "";
                                estatus = ConfigurationManager.AppSettings.Get("EstatusOk") ?? "";
                                descEstatus = ConfigurationManager.AppSettings.Get("MnsjDescEstatusOK") ?? "";
                                logger.Info(ConfigurationManager.AppSettings.Get("MnsjDocEncriptCorrectamente"));
                                logger.Info(ConfigurationManager.AppSettings.Get("EstatusOk"));
                                logger.Info(ConfigurationManager.AppSettings.Get("MnsjSinArchivos"));
                                logger.Info("Nom: Archivo " + Path.GetFileName(archivo));

                                File.WriteAllBytes(archivoEncriptado, fileOutputContent);
                                listaDatos.Add(Tuple.Create(archivo, fechaInicioProceso, estatus, mnsjExitoso, descEstatus, ruta_destino, DateTime.Now.ToString()));

                            }
                        }
                        else
                        {
                            logger.Error(ConfigurationManager.AppSettings.Get("MnsjExitoso") ?? "");
                            logger.Error(ConfigurationManager.AppSettings.Get("EstatusOkSinArchivos") ?? "");
                            logger.Error(ConfigurationManager.AppSettings.Get("MnsjSinArchivos"));

                            mnsjExitoso = ConfigurationManager.AppSettings.Get("MnsjExitoso") ?? "";
                            estatus = ConfigurationManager.AppSettings.Get("EstatusOkSinArchivos");
                            descEstatus = ConfigurationManager.AppSettings.Get("MnsjSinArchivos") ?? "";
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    descEstatusGeneralProceso = e.Message;
                    estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusError") ?? "";
                    nomArchivosProcesadosFallidos = $"\n" + archivos_encriptar?.Select(Path.GetFileName);
                    numArchivosProcesadosFallidos++;
                    logger.Error("!!!!!  nom Archivo fallido ¡¡¡¡¡ " + archivos_encriptar?.Select(Path.GetFileName));
                    logger.Error("!!!!!  Error ¡¡¡¡¡ " + e.Message);
                    Console.WriteLine("!!!!!!!!!!!!!!!!!  Error al cargar archivos ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("---------  Proceso ejecutado Correctamente ---------------");
            }
            catch (FileNotFoundException e)
            {
                descEstatusGeneralProceso = e.Message;
                estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusError") ?? "";
                logger.Error("!!!!!  Error ¡¡¡¡¡ " + e.Message);

                Console.WriteLine("¡¡¡¡¡¡¡¡¡¡¡¡ Certificado no encontrado !!!!!!!!!!!!!!!!!!!");
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("::::::::::::  Iniciamos a guardar el log :::::::::::::::");

            fechaHoraInicioFin = fechaHoraInicioFin + DateTime.Now.ToString();
            FileUtil.CreateLog(ruta_destino, numArchivosProcesar, listaDatos,
                numArchivosProcesadosFallidos, nomArchivosProcesadosFallidos,
                fechaHoraInicioFin, estatusGeneralProceso ?? "",
                descEstatusGeneralProceso, nomLogEncriptado);

        }
    }
}