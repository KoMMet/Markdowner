﻿using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using System;
using System.IO;
using System.Text;

//PM> Install-Package javascriptengineswitcher.v8.native.win-x86
//PM> Install-Package JavaScriptEngineSwitcher.V8
//PM> Install-Package marked

namespace Markdowner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("args zero");
                Console.ReadKey();
                return;
            }

            string markdown = "";
            try
            {
                markdown += File.ReadAllText(args[0], Encoding.GetEncoding("sjis"));
                var parser = new MarkdownParser(new V8JsEngine());
                var html = parser.Transform(markdown);
                File.WriteAllText(args[0] + ".html", html, Encoding.GetEncoding("sjis"));
                Console.WriteLine(html);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }

    public class MarkdownParser : IDisposable
    {
        private IJsEngine _jsEngine;
        private bool _disposed;

        public MarkdownParser(IJsEngine jsEngine)
        {
            _jsEngine = jsEngine ?? throw new ArgumentNullException("jsEngine");
            _jsEngine.ExecuteResource("Scripts.marked.js", GetType());
        }

        public string Transform(string markdown)
        {
            return _jsEngine.CallFunction<string>("marked", markdown);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_jsEngine == null) return;
            _jsEngine.Dispose();
            _jsEngine = null;
        }
    }
}
