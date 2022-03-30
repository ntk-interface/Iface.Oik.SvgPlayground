using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Iface.Oik.SvgPlayground.Util;

namespace Iface.Oik.SvgPlayground.Model
{
  public class TmStatus : TmTag
  {
    private bool         _isOn;
    private TmStatusFlag _flag;

    public bool IsOn
    {
      get => _isOn;
      set
      {
        _isOn = value;
        NotifyOnPropertyChanged();
      }
    }

    public TmStatusFlag Flag
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
    
    public bool IsFlag1   => Flag.HasFlag(TmStatusFlag.IsFlag1);
    public bool IsFlag2   => Flag.HasFlag(TmStatusFlag.IsFlag2);
    public bool IsFlag3   => Flag.HasFlag(TmStatusFlag.IsFlag3);
    public bool IsFlag4   => Flag.HasFlag(TmStatusFlag.IsFlag4);
    
    public bool IsUnacked => Flag.HasFlag(TmStatusFlag.IsUnacked);

    public bool IsMalfunction      => Flag.HasFlag(TmStatusFlag.Malfunction);
    public bool IsIntermediate     => Flag.HasFlag(TmStatusFlag.Intermediate);
    public bool IsUnderMaintenance => Flag.HasFlag(TmStatusFlag.IsUnderMaintenance);
    public bool IsUnderCheckup     => Flag.HasFlag(TmStatusFlag.IsUnderCheckup);


    public TmStatus(int ch, int rtu, int point) : base(ch, rtu, point)
    {
    }


    public IEnumerable<KeyValuePair<string, string>> FlagsList =>
      Enum.GetValues(typeof(TmStatusFlag))
          .Cast<Enum>()
          .Select(e => new KeyValuePair<string, string>(e.ToString(), e.GetDescription()));


    public string StatusCaption
    {
      get
      {
        if (IsIntermediate) return "Промеж.";
        if (IsMalfunction) return "Неиспр.";
        if (IsUnreliable) return "Недост.";
        if (IsOn) return "ВКЛ";
        return "ОТКЛ";
      }
    }
  }


  [Flags]
  public enum TmStatusFlag
  {
    [Description("-")]             None               = 0x0000,
    [Description("Недостоверно")]  Unreliable         = 0x0001,
    [Description("Уст. вручную")]  IsManuallySet      = 0x0002,
    [Description("Заблокировано")] IsManuallyBlocked  = 0x0004,
    [Description("Ненорм. режим")] IsAbnormal         = 0x0008,
    [Description("Флаг1")]         IsFlag1            = 0x0010,
    [Description("Флаг2")]         IsFlag2            = 0x0020,
    [Description("Флаг3")]         IsFlag3            = 0x0040,
    [Description("Флаг4")]         IsFlag4            = 0x0080,
    [Description("Неквитирован")]  IsUnacked          = 0x0100,
    [Description("Неисправно")]    Malfunction        = 0x1000,
    [Description("Промежуточное")] Intermediate       = 0x2000,
    [Description("Ремонт")]        IsUnderMaintenance = 0x4000,
    [Description("Контроль")]      IsUnderCheckup     = 0x8000,
  }
}