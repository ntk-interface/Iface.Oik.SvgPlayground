namespace Iface.Oik.SvgPlayground.Model
{
  // эти команды никак не будут использованы в эмуляторе, описаны для совместимости
  public enum ElementCommandType
  {
    None = 0,
    OpenSvgInThisTab,
    OpenSvgInNewTab,
    ShowTmStatus,
    Telecontrol,
    SwitchTmStatusManually,
    AckTmStatus,
    OpenTmStatusEventsArchive,
    CopyTmStatusToClipboard,
    ShowTmAnalog,
    Teleregulation,
    SetTmAnalogManually,
    OpenTmAnalogAlarms,
    OpenTmAnalogChart,
    OpenTmAnalogEventsArchive,
    CopyTmAnalogToClipboard,
  }
}