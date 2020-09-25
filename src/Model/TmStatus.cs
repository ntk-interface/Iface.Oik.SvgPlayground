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

    public bool IsUnreliable       => Flag.HasFlag(TmStatusFlag.Unreliable);
    public bool IsMalfunction      => Flag.HasFlag(TmStatusFlag.Malfunction);
    public bool IsIntermediate     => Flag.HasFlag(TmStatusFlag.Intermediate);
    public bool IsManuallySet      => Flag.HasFlag(TmStatusFlag.IsManuallySet);
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
    [Description("-")]             None               = 0,
    [Description("Недостоверный")] Unreliable         = 1,
    [Description("Неисправный")]   Malfunction        = 2,
    [Description("Промежуточное")] Intermediate       = 4,
    [Description("Уст. вручную")]  IsManuallySet      = 8,
    [Description("Ремонт")]        IsUnderMaintenance = 16,
    [Description("Контроль")]      IsUnderCheckup     = 32,
  }
}