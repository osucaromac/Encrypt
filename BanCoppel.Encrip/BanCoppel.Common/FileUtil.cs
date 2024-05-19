using System.Configuration;

namespace BanCoppel.Util
{
    public static class FileUtil
    {
        public static byte[] combineBytes(byte[] a, byte[] b)
        {
            byte[] combined = new byte[a.Length + b.Length];
            Array.Copy(a, 0, combined, 0, a.Length);
            Array.Copy(b, 0, combined, a.Length, b.Length);

            return combined;
        }

        public static void CreateLog(string rutaOrigen,int numArchivosParaProcesar, List<Tuple<string, string, string, string, string, string, string>> listaDatos,
          int numArchivosProcesadosFallidos, string nomArchivosProcesadosFallidos,
          string fechaHoraInicioFin, string estatusGeneralProceso, string descEstatusGeneralProceso,string nomLog)
        {
            // string? rutaOrigen = ConfigurationManager.AppSettings.Get("RutaLog");
            string? rutaLog = ConfigurationManager.AppSettings.Get("RutaLogWin") ?? "";


            GeneratorXML generadorXML = new GeneratorXML(rutaOrigen, numArchivosParaProcesar,
                listaDatos, numArchivosProcesadosFallidos,
                nomArchivosProcesadosFallidos, fechaHoraInicioFin, estatusGeneralProceso, descEstatusGeneralProceso, rutaLog);
            generadorXML.GenerarXML(nomLog);
        }

      
    }
}
