namespace Iface.Oik.SvgPlayground.Model
{
  // эти команды никак не будут использованы в эмуляторе, описаны для совместимости
  public enum ElementCommandType
  {
    None = 0,
    OpenDocumentInThisTab,
    OpenDocumentInNewTab,
    ShowTmStatus,
    Telecontrol,
    SwitchTmStatusManually,
    AckTmStatus,
    OpenTmStatusEventsArchive,
    CopyTmStatusToClipboard,
    ShowTmAnalog,
    Teleregulation,
    SetTmAnalogManually,
    OpenTmAnalogTechProperties,
    OpenTmAnalogAlarms,
    OpenTmAnalogChart,
    OpenTmAnalogEventsArchive,
    CopyTmAnalogToClipboard,
    OpenDocumentInOverview,
    StartProcess,
    AddTmStatusToQuickList,
    AddTmAnalogToQuickList,
    OpenTmStatusReport,
    OpenTmAnalogReport,
    CopyTmStatusNameToClipboard,
    CopyTmAnalogNameToClipboard,
    OpenDocumentInUniqueTab,
  }
}