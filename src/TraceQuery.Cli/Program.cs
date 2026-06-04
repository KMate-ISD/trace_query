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
                using TraceFileSource fileReader = new TraceFileSource(path);

                while ( null != ( lineBuffer = fileReader.GetNextLine() ) )
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