using BanCoppel.AesDencript;
using BanCoppel.RSADencript;
using BanCoppel.Util;
using log4net;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace BanCoppel.DoDecript
{

    public class DoFileDecript
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DoFileDecript));

        public static void ProcessDescrypt()
        {
            string dia = "";
            string mes = "";
            string? mnsjExitoso = "";
            string? estatus = "";
            string? descEstatus = "";
            string fechaInicioProceso = "";
            string? estatusGeneralProceso = "";
            string? descEstatusGeneralProceso = "";

            // 1. First 256 is encrypted AES key
            byte[] encryptedKey;
            // 2. Next 16 bytes for IV
            byte[] ivBytes;
            // 3. Remaining bytes is encrypted file content 
            byte[] fileBytes;
            // 4. Decrypt the AES key
            // byte[] privateKey;
            byte[] aesKeyBytes;
            // 5. Decrypt file content
            byte[] decryptedContent;

            string? mesFesDic = ConfigurationManager.AppSettings.Get("MesFesDic");
            string? diaFesDic = ConfigurationManager.AppSettings.Get("DiaFesDic");
            string? mesFesEne = ConfigurationManager.AppSettings.Get("MesFesEne");
            string? diaFesEne = ConfigurationManager.AppSettings.Get("DiaFesEne");
            string? extArchivos = ConfigurationManager.AppSettings.Get("ExtencionArchivos") ?? "";



            List<Tuple<string, string, string, string, string, string, string>> listaDatos = new List<Tuple<string, string, string, string, string, string, string>>();
            int numArchivosProcesadosFallidos = 0;
            string nomArchivosProcesadosFallidos = "";
            string fechaHoraInicioFin = "";
            int numArchivosProcesar = 0;
            RSACryptoServiceProvider RSAprivateKey;
            try
            {
                //string? ruta_origen = ConfigurationManager.AppSettings.Get("RutaArchivosEncript");
                //string? ruta_destino = ConfigurationManager.AppSettings.Get("RutaArchivosDesencript");
                string? ruta_origen = ConfigurationManager.AppSettings.Get("RutaArchivosEncriptWind") ?? "";
                string? ruta_destino = ConfigurationManager.AppSettings.Get("RutaArchivosDescriptWin") ?? "";

                string? ruta_private_key = ConfigurationManager.AppSettings.Get("RutaPrivateKeyWind") ?? "";
                //string? ruta_private_key = ConfigurationManager.AppSettings.Get("RutaPrivateKey");



                DateTime now = DateTime.Now;
                fechaHoraInicioFin = now.ToString() + " a ";
                dia = now.ToString().Substring(0, 2);
                mes = now.ToString().Substring(3, 2);

                Console.WriteLine("---------  dia:::::   " + dia);
                Console.WriteLine("---------  mes:::::   " + mes);


                try
                {
                    if ((dia.Equals(diaFesDic) && mes.Equals(mesFesDic)) || (dia.Equals(diaFesEne) && mes.Equals(mesFesEne)))
                    {

                        descEstatusGeneralProceso = ConfigurationManager.AppSettings.Get("MnsjDiaDeshabilitado") ?? "";
                        estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusDiaDeshabilitado") ?? "";
                        Console.WriteLine("---------  Execución de job deshabilitada para 25-DIC y para 01-ENE");
                    }
                    else
                    {
                        //string[] archivos_desencriptar = Directory.GetFiles(ruta_origen);
                        string[] archivos_desencriptar = Directory.GetFiles(ruta_origen, extArchivos);
                        numArchivosProcesar = archivos_desencriptar.Length;

                        if (archivos_desencriptar.Length > 0)
                        {
                            Console.WriteLine("---------  Num Doc a procesar -----  " + archivos_desencriptar.Length);

                            numArchivosProcesar = archivos_desencriptar.Length;
                            foreach (string archivo in archivos_desencriptar)
                            {
                                try
                                {
                                    fechaInicioProceso = DateTime.Now.ToString() ?? "";
                                    byte[] datosEncriptados = File.ReadAllBytes(archivo);
                                    StreamReader archivos = new StreamReader(archivo);
                                    string contenido = archivos.ReadToEnd();
                                    contenido = Encoding.UTF8.GetString(datosEncriptados);

                                    // 1. First 256 is encrypted AES key
                                    encryptedKey = datosEncriptados.Take(256).ToArray();
                                    // 2. Next 16 bytes for IV
                                    ivBytes = datosEncriptados.Skip(256).Take(16).ToArray();
                                    // 3. Remaining bytes is encrypted file content 
                                    fileBytes = datosEncriptados.Skip(encryptedKey.Length + ivBytes.Length).ToArray();
                                    // 4. Decrypt the AES key
                                    RSAprivateKey = RSADecript.ImportPrivateKey(File.ReadAllText(ruta_private_key));
                                    aesKeyBytes = RSAprivateKey.Decrypt(encryptedKey, false);
                                    // 5. Decrypt file content
                                    decryptedContent = AESDecript.DecryptFile(aesKeyBytes, ivBytes, fileBytes);
                                    contenido = Encoding.UTF8.GetString(datosEncriptados);
                                    String file = Convert.ToBase64String(datosEncriptados);

                                    //decryptedBytes = Decrypt(datosEncriptados, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(ivVector));

                                    string decryptedText = Encoding.UTF8.GetString(decryptedContent);
                                    //Guardar en una ruta especifica los archivos cifrados.
                                    string archivoDesencriptado = Path.Combine(ruta_destino, Path.GetFileName(archivo));
                                    File.WriteAllText(archivoDesencriptado, decryptedText);
                                    mnsjExitoso = ConfigurationManager.AppSettings.Get("MnsjDocEncriptCorrectamente") ?? "";
                                    estatus = ConfigurationManager.AppSettings.Get("EstatusOk") ?? "";
                                    descEstatus = ConfigurationManager.AppSettings.Get("MnsjExitoso") ?? "";

                                    logger.Info(ConfigurationManager.AppSettings.Get("MnsjDocDesencriptCorrectamente"));
                                    logger.Info(ConfigurationManager.AppSettings.Get("EstatusOk"));
                                    logger.Info(ConfigurationManager.AppSettings.Get("MnsjExitoso") ?? "");
                                    logger.Info("Nom: Archivo " + Path.GetFileName(archivo));


                                    listaDatos.Add(Tuple.Create(archivo, fechaInicioProceso, estatus, mnsjExitoso, descEstatus, archivo, DateTime.Now.ToString()));

                                }
                                catch (FileNotFoundException e)
                                {
                                    descEstatusGeneralProceso = e.Message ?? "";
                                    estatusGeneralProceso = descEstatus = ConfigurationManager.AppSettings.Get("EstatusError") ?? "";
                                    nomArchivosProcesadosFallidos = "\n" + archivos_desencriptar.Select(Path.GetFileName);
                                    numArchivosProcesadosFallidos++;

                                    logger.Error("!!!!!  nom Archivo fallido ¡¡¡¡¡ " + archivos_desencriptar.Select(Path.GetFileName));
                                    logger.Error("!!!!!  Error ¡¡¡¡¡ " + e.Message);

                                    Console.WriteLine("!!!!!!!!!!!!!!!!!  Error al cargar archivos ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡");
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("---------  sin archivos a procesar -----  " + archivos_desencriptar.Length);
                            logger.Warn("---------sin archivos a procesar---- - " + archivos_desencriptar.Length);
                            logger.Warn(ConfigurationManager.AppSettings.Get("EstatusOkSinArchivos"));
                            logger.Warn(ConfigurationManager.AppSettings.Get("MnsjExitoso") ?? "");
                            logger.Warn(ConfigurationManager.AppSettings.Get("MnsjSinArchivos"));


                            descEstatusGeneralProceso = ConfigurationManager.AppSettings.Get("MnsjSinArchivos") ?? "";
                            estatusGeneralProceso = descEstatus = ConfigurationManager.AppSettings.Get("EstatusOkSinArchivos") ?? "";
                            mnsjExitoso = ConfigurationManager.AppSettings.Get("MnsjExitoso" ?? "");
                            estatus = ConfigurationManager.AppSettings.Get("EstatusOkSinArchivos") ?? "";
                            descEstatus = ConfigurationManager.AppSettings.Get("MnsjSinArchivos") ?? "";

                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    descEstatusGeneralProceso = e.Message ?? "";
                    estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusError") ?? "";
                    logger.Error("!!!!!!!!!!!!!!!!!  Error al cargar archivos ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡");
                    logger.Error(ConfigurationManager.AppSettings.Get("EstatusError"));
                    logger.Error(e.Message);

                    Console.WriteLine("!!!!!!!!!!!!!!!!!  Error al cargar archivos ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("---------  Proceso ejecutado Correctamente ---------------");
            }
            catch (FileNotFoundException e)
            {
                descEstatusGeneralProceso = e.Message ?? "";
                estatusGeneralProceso = ConfigurationManager.AppSettings.Get("EstatusError");
                logger.Error(ConfigurationManager.AppSettings.Get("EstatusError"));
                logger.Error(e.Message);
                Console.WriteLine("¡¡¡¡¡¡¡¡¡¡¡¡ Certificado no encontrado !!!!!!!!!!!!!!!!!!!");
                Console.WriteLine(e.Message);
            }
            fechaHoraInicioFin = fechaHoraInicioFin + DateTime.Now.ToString();

            FileUtil.CreateLog(numArchivosProcesar, listaDatos,
                            numArchivosProcesadosFallidos, nomArchivosProcesadosFallidos,
                            fechaHoraInicioFin, estatusGeneralProceso ?? "",
                            descEstatusGeneralProceso);

        }



    }
}