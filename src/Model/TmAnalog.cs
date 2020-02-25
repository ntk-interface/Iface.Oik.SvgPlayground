using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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


    public bool IsUnreliable  => Flag.HasFlag(TmAnalogFlag.Unreliable);
    public bool IsAlarmLevel3 => Flag.HasFlag(TmAnalogFlag.AlarmLevel3);
    public bool IsAlarmLevel4 => Flag.HasFlag(TmAnalogFlag.AlarmLevel4);

    public string ValueString         => Value.ToString(CultureInfo.InvariantCulture);
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
    [Description("-")]                None        = 0,
    [Description("Недостоверный")]    Unreliable  = 1,
    [Description("Уставка предупр.")] AlarmLevel3 = 2,
    [Description("Уставка тревога")]  AlarmLevel4 = 4,
  }
}