using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificacionKudert
{
    class Inicio
    {
        public static string[] separadas;
        public static List<string> temas = new List<string>();
        public static List<string> min = new List<string>();
        public static List<string> diaTemas1 = new List<string>();
        public static List<string> diaMinutos1 = new List<string>();
        public static List<string> diaTemas2 = new List<string>();
        public static List<string> diaMinuto2 = new List<string>();

        static void Main(string[] args)
        {
            Console.Write("Problema: Administrar los temas de una conferencia"
                 + "\nNombre: Miguel Siguenza"
                 + "\nFecha: " + DateTime.Now.ToLocalTime() + "\n\n");

            leerArchivo();
            Console.WriteLine("--- Dia 1 ---");
            organizar(180, diaTemas1, diaMinutos1);
            organizar(240, diaTemas1, diaMinutos1);
            presentar(diaTemas1, diaMinutos1);

            Console.WriteLine("\n--- Dia 2 ---");
            organizar(180, diaTemas2, diaMinuto2);
            addTemasRestantes();
            presentar(diaTemas2, diaMinuto2);
            Console.ReadLine();
        }

        /*
         * Método encargado de leer el fichero temario.txt, permite cargar cargar los temas y minutos 
         * en listas individuales a través de la funcion Split mediante su separador "="
         */
        static void leerArchivo()
        {
            string pathToFiles = System.IO.Path.GetFullPath("./temario.txt");
            //Console.WriteLine("Ruta del Archivo Temario.txt" + "\n" + pathToFiles+"\n");
            using (StreamReader sr = new StreamReader(pathToFiles, false))
            {
                string cursor;
                while ((cursor = sr.ReadLine()) != null)
                {
                    separadas = cursor.Split('=');
                    temas.Add(separadas[0].ToString());
                    min.Add(separadas[1].ToString());
                }
            }
        }

        /*
         * Método minutosHoras recibe un parámetro encargado de convertir los minutos en su equivalente 
         * en horas, se emplea para realizar la sumatoria de hora en el método sumarHora mediante la
         * función AddTicks
         * -------- Ejemplo-----
         * min=84
         * horaDecimal = 84/60 --> 1.4
         * hora = Math.Truncate(horaDecimal) --> 1
         * minuto = (1.4 - 1)*60 --> 24
         * resultado = "01:24"
         */
        static string minutosHoras(string min)
        {
            double horaDecimal = Convert.ToDouble(min) / 60;
            int hora = Convert.ToInt32(Math.Truncate(horaDecimal));
            int minuto = Convert.ToInt32((horaDecimal - Convert.ToDouble(hora)) * 60);
            DateTime formatoHora = Convert.ToDateTime(Convert.ToString(hora) + ":" + Convert.ToString(minuto));
            var resultado = addCero(formatoHora.Hour.ToString()) + ":" + addCero(formatoHora.Minute.ToString());
            return Convert.ToString(resultado);
        }

        /*
         * Método que recibe dos parámetros de tipo DateTime varHora1 y varHora2  y retorna una cadena en formato DateTime
         * el método AddTicks propio de DateTime requiere de dos valores en formato hora para ser sumadas
         * ---------Ejemplo-----------
         * "12:45" + "00:15" = "13:00"
         */
        static string sumarHora(DateTime varHora1, DateTime varHora2)
        {
            DateTime hora = varHora1.AddTicks(varHora2.Ticks);
            var resultado = addCero(hora.Hour.ToString()) + ":" + addCero(hora.Minute.ToString());
            return resultado;
        }

        /*
         * El método addCero recibe un parámetro de tipo string misma que se evalúa su longitud
         * para proceder a añadir el número restate en caso de solo ser como valor de 1
         * ----------Ejemplo---------------
         * varHora = 5
         * return = 05
         */
        static string addCero(string varHora)
        {
            string horaFin = varHora;
            if (varHora.Length == 1)
            {
                horaFin = "0" + varHora;
            }
            return horaFin;
        }

        /*
         * Método encargado de recibir tres parámetros  uno de tipo int correspondiente a los minutos
         * y dos de tipo lista para separar los temas y minutos para cada día, el primer if separa el tema 
         * correspondiente al lightning debido que se lo agregara al final de la lista de temas y minutos
         * general debido a su menor duración y ser agregado al final.
         * Caso contrario en el else if se encargara de verificar mediante la condición si el tiempo
         * es mayor a los minutos de duración de cada tema, de cumplirse se resta los minutos al tiempo
         * procediendo a agregar el tema en el arreglo1 y el minuto en el arreglo 2.
         * Una vez agregado los temas y minutos en su arreglo respectivo se eliminar el tema y minuto de 
         * la lista general.
         * La variable aux guarda el tiempo que se recibe al inicio para no perder su valor
         * mismo que será evaluado en el if para agregar el evento social al final de la jornada.
         */
        static void organizar(int tiempo, List<string> arreglo, List<string> arreglo1)
        {
            int aux = tiempo;
            for (int i = 0; i < temas.Count(); i++)
            {
                if (min[i].ToString().Equals("5"))
                {
                    temas.Add(temas[i].ToString());
                    min.Add(min[i].ToString());
                    temas.RemoveAt(i);
                    min.RemoveAt(i);
                }
                else if (tiempo >= Convert.ToInt32(min[i].ToString()))
                {
                    tiempo = tiempo - Convert.ToInt32(min[i].ToString());
                    arreglo.Add(temas[i]);
                    arreglo1.Add(min[i]);
                    temas.RemoveAt(i);
                    min.RemoveAt(i);
                }
            }
            if (aux == 240)
            {
                arreglo.Add("Networking Event");
                arreglo1.Add("0");
            }
        }

        /*
         * Método encargado de añadir los temas sobrantes para la seunda jornada del día 2 
         * en los arreglos respectivos, al final se agrega el evento social con una duración
         * indefinida
         */
        static void addTemasRestantes()
        {
            for (int i = 0; i < temas.Count(); i++)
            {
                diaTemas2.Add(temas[i]);
                diaMinuto2.Add(min[i]);
            }
            diaTemas2.Add("Networking Event");
            diaMinuto2.Add("0");
        }

        /*
         * Permite realizar la visualizacion de los temas del día 1 y 2 respectivamente
         * mediante los dos parametros de tipo List que recibe, la variable hora es 
         * necesaria establecerla para el inicio de la jornada por día.
         * 
         * El bucle for permite recorrer la lista a presentar, la condición if permite separar 
         * la presentacion para agregar el espacio del Almuerzo con una duracion de 60 minutos,
         * para proceder a presentar el siguiente tema siendo necesario hacer uso del método 
         * sumarHora para continuar la secuencia de "12:00" + "01:00" = "13:00" para el inicio del
         * tema a continuación.
         * 
         * El else if evalúa la igual de la cadena correspondiente al Networking Event para 
         * no presentar el detalle respecto al tiempo de duración debido que es indefinido.
         * 
         * Caso contrario permite presentar la hora inical de la jornada del día concatenando
         * el tema correspondiente más la respeciva duración del mismo como detalle
         * a continuación se procede a sumar la hora inical más el tiempo de duración del tema
         * para ser reemplazada por la variable hora.
         */
        static void presentar(List<string> arreglo, List<string> arreglo1)
        {
            string hora = "09:00";
            for (int i = 0; i < arreglo.Count(); i++)
            {
                if (hora.Equals("12:00"))
                {
                    Console.WriteLine(hora + "  Almuerzo");
                    hora = sumarHora(Convert.ToDateTime(hora), Convert.ToDateTime(minutosHoras("60")));
                    Console.WriteLine(hora + "  " + arreglo[i] + " " + arreglo1[i] + "min");
                    hora = sumarHora(Convert.ToDateTime(hora), Convert.ToDateTime(minutosHoras(arreglo1[i])));
                }
                else if (arreglo[i].Equals("Networking Event"))
                {
                    Console.WriteLine(hora + "  " + arreglo[i]);
                }
                else
                {
                    Console.WriteLine(hora + "  " + arreglo[i] + " " + arreglo1[i] + "min");
                    hora = sumarHora(Convert.ToDateTime(hora), Convert.ToDateTime(minutosHoras(arreglo1[i])));
                }
            }
            //Console.WriteLine("17:00" + "  Networking Event");
        }
    }
}
