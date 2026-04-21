using CommunityToolkit.Mvvm.ComponentModel;

namespace Iface.Oik.SvgPlayground.Model;

public class TmTag : ObservableObject
{
  public int Ch    { get; }
  public int Rtu   { get; }
  public int Point { get; }


  public TmTag(int ch, int rtu, int point)
  {
    Ch    = ch;
    Rtu   = rtu;
    Point = point;
  }


  public override string ToString()
  {
    string prefix;
    switch (this)
    {
      case TmStatus _:
        prefix = "#TC";
        break;
      case TmAnalog _:
        prefix = "#TT";
        break;
      default:
        return string.Empty;
    }
    return $"{prefix}{Ch}:{Rtu}:{Point}";
  }


  public bool AddrEquals(int ch, int rtu, int point)
  {
    return Ch    == ch  &&
           Rtu   == rtu &&
           Point == point;
  }
}