using BanCoppel.Logger;
using System.Configuration;

namespace BanCoppel.Util
{
    public static class FileUtil
    {
        public static void Main()
        {
        }

            public static byte[] combineBytes(byte[] a, byte[] b)
            {
            byte[] combined = new byte[a.Length + b.Length];
            Array.Copy(a, 0, combined, 0, a.Length);
            Array.Copy(b, 0, combined, a.Length, b.Length);

            return combined;
        }

        public static void CreateLog(int numArchivosParaProcesar, List<Tuple<string, string, string, string, string, string, string>> listaDatos,
          int numArchivosProcesadosFallidos, string nomArchivosProcesadosFallidos,
          string fechaHoraInicioFin, string estatusGeneralProceso, string descEstatusGeneralProceso)
        {
            // string? rutaOrigen = ConfigurationManager.AppSettings.Get("RutaLogWin");
            string? rutaOrigen = ConfigurationManager.AppSettings.Get("RutaLog") ?? "";

            GeneradorXML generadorXML = new GeneradorXML(rutaOrigen, numArchivosParaProcesar,
                listaDatos, numArchivosProcesadosFallidos,
                nomArchivosProcesadosFallidos, fechaHoraInicioFin, estatusGeneralProceso, descEstatusGeneralProceso);
            generadorXML.GenerarXML("logEncriptacion");
        }


    }
}
