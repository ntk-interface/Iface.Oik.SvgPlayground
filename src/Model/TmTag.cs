using CommunityToolkit.Mvvm.ComponentModel;

namespace Iface.Oik.SvgPlayground.Model;

public abstract class TmTag : ObservableObject
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


  protected abstract string ToStringPrefix { get; }


  public override string ToString()
  {
    return $"{ToStringPrefix}{Ch}:{Rtu}:{Point}";
  }


  public bool AddrEquals(int ch, int rtu, int point)
  {
    return Ch    == ch  &&
           Rtu   == rtu &&
           Point == point;
  }
}