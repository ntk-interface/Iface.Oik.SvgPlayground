using System.ComponentModel;

namespace Iface.Oik.SvgPlayground.Model;


public enum ElementCommandType
{
  None = 0,
  [Description("Открыть в тек. вкладке")]     OpenDocumentInThisTab,
  [Description("Открыть в новой вкладке")]    OpenDocumentInNewTab,
  [Description("Открыть окно ТС")]            ShowTmStatus,
  [Description("Команда ТУ")]                 Telecontrol,
  [Description("Команда перекл. вручную")]    SwitchTmStatusManually,
  [Description("Команда квитировать")]        AckTmStatus,
  [Description("Открыть архив событий")]      OpenTmStatusEventsArchive,
  [Description("Копировать адрес в буфер")]   CopyTmStatusToClipboard,
  [Description("Открыть окно ТС")]            ShowTmAnalog,
  [Description("Команда регулирования")]      Teleregulation,
  [Description("Команда установить вручную")] SetTmAnalogManually,
  [Description("Открыть техпараметры")]       OpenTmAnalogTechProperties,
  [Description("Открыть уставки")]            OpenTmAnalogAlarms,
  [Description("Открыть график")]             OpenTmAnalogChart,
  [Description("Открыть архив событий")]      OpenTmAnalogEventsArchive,
  [Description("Копировать адрес в буфер")]   CopyTmAnalogToClipboard,
  [Description("Открыть во вспл. окне")]      OpenDocumentInOverview,
  [Description("Запустить процесс")]          StartProcess,
  [Description("Добавить адрес в список")]    AddTmStatusToQuickList,
  [Description("Добавить адрес в список")]    AddTmAnalogToQuickList,
  [Description("Открыть отчет")]              OpenTmStatusReport,
  [Description("Открыть отчет")]              OpenTmAnalogReport,
  [Description("Копировать имя в буфер")]     CopyTmStatusNameToClipboard,
  [Description("Копировать имя в буфер")]     CopyTmAnalogNameToClipboard,
  [Description("Открыть в уник. вкладке")]    OpenDocumentInUniqueTab,
  [Description("Открыть видео в окне")]       OpenVideoInOverview,
  [Description("Открыть график")]             OpenTmStatusChart,
}


public enum ElementCommandLevel
{
  [Description("Конт. меню")] ContextMenu = 0,
  [Description("По нажатию")] Click       = 1,
}


public class ElementCommand
{
  public ElementCommandLevel Level           { get; }
  public ElementCommandType  Type            { get; }
  public int                 ParameterInt    { get; }
  public string              ParameterString { get; }
  public string              Caption         { get; }


  public ElementCommand(ElementCommandLevel level, ElementCommandType type)
  {
    Level = level;
    Type  = type;
  }


  public ElementCommand(ElementCommandLevel level,
                        ElementCommandType  type,
                        int                 parameterInt,
                        string              caption = null)
    : this(level, type)
  {
    ParameterInt    = parameterInt;
    ParameterString = string.Empty;
    Caption         = caption;
  }


  public ElementCommand(ElementCommandLevel level,
                        ElementCommandType  type,
                        string              parameterString,
                        string              caption = null)
    : this(level, type)
  {
    ParameterString = parameterString;
    Caption         = caption;
  }
}