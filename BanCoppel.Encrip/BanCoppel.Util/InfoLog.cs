using System.Xml.Linq;

namespace BanCoppel.Util
{
    public class GeneratorXML
    {
        private string rutaOrigen;
        private int numArchivosParaProcesar;
        private List<Tuple<string, string, string, string, string, string, string>> listaDatos;
        private int numeroArchivosFallidos;
        private string nombreArchivosFallidos;
        private string fechaHoraInicioFin;
        string estatusGeneralProceso;
        string descEstatusGeneralProceso;
        string rutaDestinoLog;

        public GeneratorXML(string rutaOrigen, int numArchivosParaProcesar,
            List<Tuple<string, string, string, string, string, string, string>> listaDatos,
            int numArchivosProcesadosFallidos, string nomArchivosProcesadosFallidos,
            string fechaHoraInicioFin, string estatusGeneralProceso, string descEstatusGeneralProceso, string rutaDestinoLog) // Nuevo parámetro para la ruta de destino
            {
            this.rutaOrigen = rutaOrigen;
            this.numArchivosParaProcesar = numArchivosParaProcesar;
            this.listaDatos = listaDatos;
            this.numeroArchivosFallidos = numArchivosProcesadosFallidos;
            this.nombreArchivosFallidos = nomArchivosProcesadosFallidos;
            this.fechaHoraInicioFin = fechaHoraInicioFin;
            this.estatusGeneralProceso = estatusGeneralProceso;
            this.descEstatusGeneralProceso = descEstatusGeneralProceso;
            this.rutaDestinoLog = rutaDestinoLog;
        }

        public void GenerarXML(string nombreArchivo)
        {
            XDocument doc = new XDocument(
                new XElement("Reporte",
                    new XElement("ruta_origen", rutaOrigen),
                    new XElement("num_archivos_para_procesar", numArchivosParaProcesar),
                    new XElement("ArchivosFallidos", numeroArchivosFallidos),
                    new XElement("DescArchivosFallidos", nombreArchivosFallidos),
                    new XElement("fecha_hora_inicio_fin", fechaHoraInicioFin),
                    new XElement("Estatus_General_Proceso", estatusGeneralProceso),
                    new XElement("Desc_Estatus_General_Proceso", descEstatusGeneralProceso)
                )
            );

            foreach (var dato in listaDatos)
            {
                XElement reporte = new XElement("Detalles",
                    new XElement("NombreArchivo", dato.Item1),
                    new XElement("HoraInicioProceso", dato.Item2),
                    new XElement("EstatusCode", dato.Item3),
                    new XElement("DescEstatus", dato.Item4),
                    new XElement("Descripcion", dato.Item5),
                    new XElement("RutaDestino", dato.Item6),
                    new XElement("HoraInicio-Fin", dato.Item7)
                );
                doc.Root?.Add(reporte);
            }

            string fechaActual = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss"); // Agrega hora, minuto y segundo al nombre del archivo
            string nombreArchivoCompleto = Path.Combine(rutaDestinoLog, $"{nombreArchivo}_{fechaActual}.xml"); // Combina la ruta de destino con el nombre del archivo
            doc.Save(nombreArchivoCompleto);
        }
    }
}

/*  Área: Crédito y Cobranza
 *  Descripción: Genera XML con registro de Logs
 *  Autor: Eduardo Castillo Hernández
 *  Versión: .Net 7.0
 */