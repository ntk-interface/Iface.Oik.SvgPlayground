namespace Iface.Oik.SvgPlayground.Model
{
  public class TmTag
  {
    public int Ch    { get; set; }
    public int Rtu   { get; set; }
    public int Point { get; set; }


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
  }
}