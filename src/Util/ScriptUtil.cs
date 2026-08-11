using System;
using System.Linq;

namespace Iface.Oik.SvgPlayground.Util;

public static class ScriptUtil
{
  private const string Separator = "|-_-|";
  

  public static string JoinFunctionNameArguments(string functionName, string[] arguments)
  {
    if (arguments == null || arguments.Length == 0)
    {
      return functionName;
    }
    return $"{functionName}{Separator}{string.Join(Separator, arguments)}";
  }

  
  public static (string FunctionName, string[] Arguments) SplitFunctionArguments(string source)
  {
    if (string.IsNullOrEmpty(source))
    {
      return (string.Empty, Array.Empty<string>());
    }

    var parts = source.Split(new[] { Separator }, StringSplitOptions.None);
    return (parts.FirstOrDefault() ?? string.Empty, parts.Skip(1).ToArray());
  }
}
