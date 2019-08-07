/*
 * Creado por SharpDevelop.
 * Usuario: ggonzalez
 * Fecha: 14/10/2008
 * Hora: 08:41 a.m.
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Corrector
{
	[Serializable]
	public class Rama{
		public bool fin=false;
		public Rama[] letra=new Rama[33];
	}
	public class Arbol
    {
		Rama ltr;
        StreamReader sr;
        IFormatter formateador;
        Stream flujo;
		public Arbol(String ruta)
		{
			try{
				formateador = new BinaryFormatter();
        		flujo = new FileStream("Arbol.bin", FileMode.Create, FileAccess.Write);
				sr = new StreamReader(ruta);
	            String diccionario = sr.ReadToEnd();
				ltr=new Rama();
				String[] lineas=diccionario.Split('\n');
				String[] linea;
				for(int i=0;i<lineas.Length;i++)
	            {
					linea=lineas[i].Split(',');
					Console.WriteLine(linea[1]);
	                inserta(ltr, linea[1], 0);
				}
	            guardar(ltr);
			}catch(Exception e){
				Console.WriteLine(e);
			}finally{
				flujo.Close();
			}
		}
        public Rama dic() 
        {
            return ltr;
        }
        public void inserta(Rama ltr,String pal,int letra) 
        {
            int aux = compara(pal[letra]);
            if (ltr.letra[aux] == null)
                ltr.letra[aux] = new Rama();
            if(letra == pal.Length-1)
                ltr.letra[aux].fin = true;
            if(letra!=pal.Length-1)
                inserta(ltr.letra[aux], pal, letra + 1);
        }
        public int compara(char letra) 
        {
            if (letra >= 'a' && letra <= 'z')
                return letra - 'a';
            if (letra >= 'A' && letra <= 'Z')
                return letra - 'A';
            if (letra == 'ñ' || letra == 'Ñ')
                return 26;
            if (letra == 'á' || letra == 'Á')
                return 27;
            if (letra == 'é' || letra == 'É')
                return 28;
            if (letra == 'í' || letra == 'Í')
                return 29;
            if (letra == 'ó' || letra == 'Ó')
                return 30;
            if (letra == 'ú' || letra == 'Ú')
                return 31;
            return -1;
        }
        void guardar(Rama ltr)
        {
            if (ltr != null)
            {
                try
                {
                    formateador.Serialize(flujo, ltr);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Falla al serializar. Motivo: " + e.Message);
                }
            }
        }
	}
    class Corregir
    {
        Rama ltr;
        Stream flujo;
        IFormatter formateador = new BinaryFormatter();
        public Corregir() 
        {
        	try{
        		flujo = new FileStream("Arbol.bin", FileMode.Open, FileAccess.Read);
	            if (ltr == null)
	            {
	                try
	                {
	                    ltr=(Rama)formateador.Deserialize(flujo);
	                    corregirTexto();
	                }
	                catch (SerializationException e)
	                {
	                    Console.WriteLine("Falla al deserializar. Motivo: " + e.Message);
	                }
	                //for (int i = 0; i < 33; i++)
	                //    guardar(ltr.letra[i]);
	            }
	            flujo.Close();
            }catch(Exception e){
        		Console.WriteLine(e);
        	}finally{
        		
        	}
        }
        public int compara(char letra)
        {
            if (letra >= 'a' && letra <= 'z')
                return letra - 'a';
            if (letra >= 'A' && letra <= 'Z')
                return letra - 'A';
            if (letra == 'ñ' || letra == 'Ñ')
                return 26;
            if (letra == 'á' || letra == 'Á')
                return 27;
            if (letra == 'é' || letra == 'É')
                return 28;
            if (letra == 'í' || letra == 'Í')
                return 29;
            if (letra == 'ó' || letra == 'Ó')
                return 30;
            if (letra == 'ú' || letra == 'Ú')
                return 31;
            return -1;
        }
        public void corregirTexto()
        {
            StreamReader sr;
            try{
            	sr=new StreamReader("texto.txt");
            	String texto = sr.ReadToEnd();
            	analizar(texto);
            }catch(FileNotFoundException e){
            	Console.WriteLine(e);
            }
        }
        public void analizar(String texto) 
        {
            int c,cp,j;
            int[] tkn=new int[30];
            char[] pal = new char[30];
            String palabra;
            StreamWriter errores=new StreamWriter("errores.txt");
            tkn[0] = -1;
            cp=0;
            for (int i = 0; i < texto.Length;i++ )
            {
                c=compara(texto[i]);
                if (c == -1)
                {
                    if (tkn[0] != -1)
                    {
                        palabra = new String(pal);
                        if (busqueda(ltr, tkn, cp, 0))
                            Console.Write("Palabra encontrada ");
                        else
                        {
                            Console.Write("Palabra no encontrada ");
                            errores.WriteLine(palabra);
                        }
                        Console.WriteLine(palabra);
                    }
                    cp = 0;
                    tkn[0] = -1;
                    for (j = 0; j < 30;j++ )
                        pal[j] = '\0';
                }
                else 
                {
	            	tkn[cp] = c;
	                pal[cp] = texto[i];
	                cp++;
	                tkn[cp] = -1;
	                pal[cp] = '\0';
                }
            }
        }
        public bool busqueda(Rama ltr,int[] tkn,int lp,int cp) 
        {
            if (lp == cp)
                return ltr.fin;
            if (ltr.letra[tkn[cp]] != null)
                return busqueda(ltr.letra[tkn[cp]],tkn,lp,cp+1);
            return false;
        }
    }
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Corrector Ortográfico");
            bool ciclar=true;
            String opc;
            Arbol A;
            Corregir C;
            do
            {
                Console.WriteLine("1.- Crear Arbol");
                Console.WriteLine("2.- Analizar Texto");
                Console.WriteLine("3.- Salir");
                Console.Write("opc: ");
                opc=Console.ReadLine();
                if (opc == "1")
                {
                    A = new Arbol("lemas.txt");
                    Rama dic = A.dic();
                    Console.WriteLine("Arbol Guardado Exitosamente");
                }
                if (opc == "2") 
                {
                    C = new Corregir();
                }
                if (opc == "3") 
                {
                    ciclar = false;
                }
			}while(ciclar);
		}
	}
}