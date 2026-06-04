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

            // Temporary cli-core wiring.
            // TODO: Rework on deliberate implementation.
            try
            {
                using TraceFileSource traceFileSource = new TraceFileSource(path);

                Byte lineCount = 0;
                while ( null != ( lineBuffer = traceFileSource.GetNextLine() ) )
                {
                    ++lineCount;
                }
                Console.WriteLine(string.Format("[{0}] {1} line(s) read.", path, lineCount));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}