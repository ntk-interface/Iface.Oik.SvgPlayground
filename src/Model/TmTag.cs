using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Iface.Oik.SvgPlayground.Model
{
  public class TmTag : INotifyPropertyChanged
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


    public bool AddrEquals(int ch, int rtu, int point)
    {
      return Ch    == ch  &&
             Rtu   == rtu &&
             Point == point;
    }


    public event PropertyChangedEventHandler PropertyChanged;


    protected void NotifyOnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}