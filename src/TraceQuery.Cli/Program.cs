using System;
using TraceQuery.Core.Ingestion;

internal class Program
{
    private static void Main(string[] args)
    {
        if ( 0 < args.Length )
        {
            String path = args[0];
            String? lineBuffer;

            try
            {
                using FileReader fileReader = new FileReader(path);

                while ( null != ( lineBuffer = fileReader.RouteLine() ) )
                {
                    Console.WriteLine(lineBuffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}