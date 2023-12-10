using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilverCodeGenerator.CodeGenerators
{
    internal interface ICodeGenerator
    {
        bool IsReady { get; }
        Task<string> Generate(string input);
    }
}
