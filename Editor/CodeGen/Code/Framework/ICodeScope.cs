namespace Framework
{
    using System.Collections.Generic;

    public interface ICodeScope : ICode
    {
        List<ICode> Codes { get; set; }
    }
}