using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Iface.Oik.SvgPlayground.Util;

namespace Iface.Oik.SvgPlayground.Model
{
  public class TmAnalog : TmTag
  {
    public static readonly string InvalidValueString = "???";


    private TmAnalogFlag _flag;
    private float        _value;
    private string       _unit;

    
    public float Value
    {
      get => _value;
      set
      {
        _value = value;
        NotifyOnPropertyChanged();
      }
    }

    
    public string Unit
    {
      get => _unit;
      set
      {
        _unit = value;
        NotifyOnPropertyChanged();
      }
    }


    public TmAnalogFlag Flag
    {
      get => _flag;
      set
      {
        _flag = value;
        NotifyOnPropertyChanged();
      }
    }


    public bool IsUnreliable => Flag.HasFlag(TmAnalogFlag.Unreliable);

    public string ValueWithUnitString => $"{Value} {Unit}";


    public TmAnalog(int ch, int rtu, int point) : base(ch, rtu, point)
    {
    }


    public IEnumerable<KeyValuePair<string, string>> FlagsList =>
      Enum.GetValues(typeof(TmAnalogFlag))
          .Cast<Enum>()
          .Select(e => new KeyValuePair<string, string>(e.ToString(), e.GetDescription()));
  }


  [Flags]
  public enum TmAnalogFlag
  {
    [Description("-")]             None       = 0,
    [Description("Недостоверный")] Unreliable = 1,
  }
}