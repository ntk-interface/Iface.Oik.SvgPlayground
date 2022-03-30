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


    public bool IsUnreliable      => Flag.HasFlag(TmStatusFlag.Unreliable);
    public bool IsManuallySet     => Flag.HasFlag(TmStatusFlag.IsManuallySet);
    public bool IsManuallyBlocked => Flag.HasFlag(TmStatusFlag.IsManuallyBlocked);
    public bool IsAbnormal        => Flag.HasFlag(TmStatusFlag.IsAbnormal);
    
    public bool IsAlarmLevel1 => Flag.HasFlag(TmAnalogFlag.IsAlarmLevel1);
    public bool IsAlarmLevel2 => Flag.HasFlag(TmAnalogFlag.IsAlarmLevel2);
    public bool IsAlarmLevel3 => Flag.HasFlag(TmAnalogFlag.IsAlarmLevel3);
    public bool IsAlarmLevel4 => Flag.HasFlag(TmAnalogFlag.IsAlarmLevel4);
    
    public bool IsUnacked => Flag.HasFlag(TmStatusFlag.IsUnacked);

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
    [Description("-")]                None              = 0x0000,
    [Description("Недостоверно")]     Unreliable        = 0x0001,
    [Description("Уст. вручную")]     IsManuallySet     = 0x0002,
    [Description("Заблокировано")]    IsManuallyBlocked = 0x0004,
    [Description("Ненорм. режим")]    IsAbnormal        = 0x0008,
    [Description("Уставка ОС")]       IsAlarmLevel1     = 0x0010,
    [Description("Уставка ПС2")]      IsAlarmLevel2     = 0x0020,
    [Description("Уставка ПС1")]      IsAlarmLevel3     = 0x0040,
    [Description("Уставка АС")]       IsAlarmLevel4     = 0x0080,
  }
}