using System;
using System.Collections.Generic;
using System.Drawing;
using Iface.Oik.SvgPlayground.MainWindow;
using Iface.Oik.SvgPlayground.Util;
using Jint;
using Jint.Parser;
using Svg;
using Svg.Transforms;

namespace Iface.Oik.SvgPlayground.Model;

public class Element
{
  private readonly MainWindowViewModel _owner;
  private readonly SvgVisualElement    _svgElement;

  private readonly string                          _script;
  private readonly Engine                          _scriptEngine;
  private readonly IDictionary<string, SvgElement> _elementsWithId;
  private readonly bool                            _isUpdatable;
  private readonly RectangleF                      _bound;

  public ElementCommand       ClickCommand        { get; private set; }
  public List<ElementCommand> ContextMenuCommands { get; } = new();
  public bool                 IsInit              { get; }
  public bool                 IsTickEnabled       { get; }
  
  
  public string CustomToolTip { get; set; }


  private Element(MainWindowViewModel owner,
                  SvgVisualElement    svgElement)
  {
    _owner      = owner;
    _svgElement = svgElement;

    var scriptElement = SvgUtil.FindDescriptionElement(_svgElement);
    if (scriptElement == null)
    {
      return;
    }
    _script = scriptElement.Content;

    _elementsWithId = new Dictionary<string, SvgElement>(SvgUtil.GetElementsDictionaryWithAttribute(_svgElement,
                                                           "oikid"));

    _scriptEngine = new Engine();

    try
    {
      _scriptEngine
       .SetValue("overrideUpdateInterval",           new Action<int>(_owner.OverrideUpdateInterval))
       .SetValue("overrideTickInterval",             new Action<int>(_owner.OverrideTickInterval))
       .SetValue("logMessage",                       new Action<string>(LogMessage))
       .SetValue("setElementProperty",               new Action<string, string, string>(SetElementProperty))
       .SetValue("getElementProperty",               new Func<string, string, string>(GetElementProperty))
       .SetValue("initTmStatus",                     new Func<int, int, int, string, int>(_owner.InitTmStatus))
       .SetValue("initTmAnalog",                     new Func<int, int, int, string, int>(_owner.InitTmAnalog))
       .SetValue("initVariable",                     new Func<string, string, int>(_owner.InitVariable))
       .SetValue("getTmStatusStatus",                new Func<int, int>(_owner.GetTmStatusStatus))
       .SetValue("getTmStatusCaption",               new Func<int, string>(_owner.GetTmStatusCaption))
       .SetValue("isTmStatusUnreliable",             new Func<int, bool>(_owner.IsTmStatusUnreliable))
       .SetValue("isTmStatusMalfunction",            new Func<int, bool>(_owner.IsTmStatusMalfunction))
       .SetValue("isTmStatusIntermediate",           new Func<int, bool>(_owner.IsTmStatusIntermediate))
       .SetValue("isTmStatusManuallySet",            new Func<int, bool>(_owner.IsTmStatusManuallySet))
       .SetValue("isTmStatusManuallyBlocked",        new Func<int, bool>(_owner.IsTmStatusManuallyBlocked))
       .SetValue("isTmStatusAbnormal",               new Func<int, bool>(_owner.IsTmStatusAbnormal))
       .SetValue("isTmStatusUnacked",                new Func<int, bool>(_owner.IsTmStatusUnacked))
       .SetValue("isTmStatusCustomFlagRaised",       new Func<int, int, bool>(_owner.IsTmStatusCustomFlagRaised))
       .SetValue("isTmStatusUnderMaintenance",       new Func<int, bool>(_owner.IsTmStatusUnderMaintenance))
       .SetValue("isTmStatusUnderCheckup",           new Func<int, bool>(_owner.IsTmStatusUnderCheckup))
       .SetValue("isTmStatusOn",                     new Func<int, bool>(_owner.IsTmStatusOn))
       .SetValue("getTmStatusCustomFlagCaption",     new Func<int, int, string>(_owner.GetTmStatusCustomFlagCaption))
       .SetValue("getTmAnalogValue",                 new Func<int, float>(_owner.GetTmAnalogValue))
       .SetValue("getTmAnalogUnit",                  new Func<int, string>(_owner.GetTmAnalogUnit))
       .SetValue("getTmAnalogValueString",           new Func<int, string>(_owner.GetTmAnalogValueString))
       .SetValue("getTmAnalogValueWithUnitString",   new Func<int, string>(_owner.GetTmAnalogValueWithUnitString))
       .SetValue("isTmAnalogUnreliable",             new Func<int, bool>(_owner.IsTmAnalogUnreliable))
       .SetValue("isTmAnalogManuallySet",            new Func<int, bool>(_owner.IsTmAnalogManuallySet))
       .SetValue("isTmAnalogManuallyBlocked",        new Func<int, bool>(_owner.IsTmAnalogManuallyBlocked))
       .SetValue("isTmAnalogAbnormal",               new Func<int, bool>(_owner.IsTmAnalogAbnormal))
       .SetValue("isTmAnalogUnacked",                new Func<int, bool>(_owner.IsTmAnalogUnacked))
       .SetValue("isTmAnalogAlarmAlert",             new Func<int, bool>(_owner.IsTmAnalogAlarmAlert))
       .SetValue("isTmAnalogAlarmWarning",           new Func<int, bool>(_owner.IsTmAnalogAlarmWarning))
       .SetValue("isTmAnalogAlarmFlagRaised",        new Func<int, int, bool>(_owner.IsTmAnalogAlarmFlagRaised))
       .SetValue("isVariableUnreliable",             new Func<int, bool>(_owner.IsVariableUnreliable))
       .SetValue("isVariableManuallySet",            new Func<int, bool>(_owner.IsVariableManuallySet))
       .SetValue("isVariableOn",                     new Func<int, bool>(_owner.IsVariableOn))
       .SetValue("setVariable",                      new Action<int, bool>(_owner.SetVariable))
       .SetValue("setVariableUnreliable",            new Action<int, bool>(_owner.SetVariableUnreliable))
       .SetValue("setVariableManuallySet",           new Action<int, bool>(_owner.SetVariableManuallySet))
       .SetValue("initTmStatusDefaultCommands",      new Action<int>(InitTmStatusDefaultCommands))
       .SetValue("initTmAnalogDefaultCommands",      new Action<int>(InitTmAnalogDefaultCommands))
       .SetValue("initOpenDocumentDefaultCommands",  new Action<string>(InitOpenDocumentDefaultCommands))
       .SetValue("initCustomToolTip",                new Action<string>(InitCustomToolTip))
       .SetValue("initClickIntCommand",              new Action<int, int>(InitClickCommand))
       .SetValue("initContextMenuIntCommand",        new Action<int, int, string>(InitContextMenuCommand))
       .SetValue("initClickStringCommand",           new Action<int, string>(InitClickCommand))
       .SetValue("initContextMenuStringCommand",     new Action<int, string, string>(InitContextMenuCommand))
       .SetValue("initContextMenuSeparator",         new Action(InitContextMenuSeparator))
       .SetValue("NONE",                             ElementCommandType.None)
       .SetValue("OPEN_DOCUMENT_IN_THIS_TAB",        ElementCommandType.OpenDocumentInThisTab)
       .SetValue("OPEN_DOCUMENT_IN_NEW_TAB",         ElementCommandType.OpenDocumentInNewTab)
       .SetValue("OPEN_DOCUMENT_IN_UNIQUE_TAB",      ElementCommandType.OpenDocumentInUniqueTab)
       .SetValue("OPEN_DOCUMENT_IN_OVERVIEW",        ElementCommandType.OpenDocumentInOverview)
       .SetValue("SHOW_TM_STATUS",                   ElementCommandType.ShowTmStatus)
       .SetValue("TELECONTROL",                      ElementCommandType.Telecontrol)
       .SetValue("SWITCH_TM_STATUS_MANUALLY",        ElementCommandType.SwitchTmStatusManually)
       .SetValue("ACK_TM_STATUS",                    ElementCommandType.AckTmStatus)
       .SetValue("OPEN_TM_STATUS_EVENTS_ARCHIVE",    ElementCommandType.OpenTmStatusEventsArchive)
       .SetValue("OPEN_TM_STATUS_REPORT",            ElementCommandType.OpenTmStatusReport)
       .SetValue("COPY_TM_STATUS_TO_CLIPBOARD",      ElementCommandType.CopyTmStatusToClipboard)
       .SetValue("COPY_TM_STATUS_NAME_TO_CLIPBOARD", ElementCommandType.CopyTmStatusNameToClipboard)
       .SetValue("ADD_TM_STATUS_TO_QUICK_LIST",      ElementCommandType.AddTmStatusToQuickList)
       .SetValue("SHOW_TM_ANALOG",                   ElementCommandType.ShowTmAnalog)
       .SetValue("TELEREGULATE",                     ElementCommandType.Teleregulation)
       .SetValue("SET_TM_ANALOG_MANUALLY",           ElementCommandType.SetTmAnalogManually)
       .SetValue("OPEN_TM_ANALOG_TECH_PROPERTIES",   ElementCommandType.OpenTmAnalogTechProperties)
       .SetValue("OPEN_TM_ANALOG_ALARMS",            ElementCommandType.OpenTmAnalogAlarms)
       .SetValue("OPEN_TM_ANALOG_CHART",             ElementCommandType.OpenTmAnalogChart)
       .SetValue("OPEN_TM_ANALOG_EVENTS_ARCHIVE",    ElementCommandType.OpenTmAnalogEventsArchive)
       .SetValue("OPEN_TM_ANALOG_REPORT",            ElementCommandType.OpenTmAnalogReport)
       .SetValue("COPY_TM_ANALOG_NAME_TO_CLIPBOARD", ElementCommandType.CopyTmAnalogNameToClipboard)
       .SetValue("COPY_TM_ANALOG_TO_CLIPBOARD",      ElementCommandType.CopyTmAnalogToClipboard)
       .SetValue("ADD_TM_ANALOG_TO_QUICK_LIST",      ElementCommandType.AddTmAnalogToQuickList)
       .SetValue("START_PROCESS",                    ElementCommandType.StartProcess)
       .SetValue("OPEN_VIDEO_IN_OVERVIEW",           ElementCommandType.OpenVideoInOverview)
       .SetValue("OPEN_TM_STATUS_CHART",             ElementCommandType.OpenTmStatusChart)
       .Execute(_script, new ParserOptions { Tolerant = true });
    }
    catch (Exception ex)
    {
      throw new Exception(ToString() + Environment.NewLine + ex.Message);
    }

    _isUpdatable  = _scriptEngine.Global.HasProperty("update");
    IsTickEnabled = _scriptEngine.Global.HasProperty("tick");

    _bound = GetSvgElementBound();

    IsInit = true;
  }


  public static Element Create(MainWindowViewModel owner, SvgElement rootElement)
  {
    if (owner == null ||
        !(rootElement is SvgVisualElement visualRootElement))
    {
      return null;
    }

    var oikSvgElement = new Element(owner, visualRootElement);

    return (oikSvgElement.IsInit) ? oikSvgElement : null;
  }


  public void Update()
  {
    if (!_isUpdatable) return;

    try
    {
      _scriptEngine.Invoke("update");
    }
    catch (Exception ex)
    {
      LogMessage("Ошибка выполнения циклической функции скрипта дорасчета: " + ex.Message);
    }
  }


  public void InvokeTick()
  {
    if (!IsTickEnabled) return;

    try
    {
      _scriptEngine.Invoke("tick");
    }
    catch (Exception ex)
    {
      LogMessage("Ошибка выполнения циклической функции скрипта дорасчета: " + ex.Message);
    }
  }


  public bool BoundContains(float x, float y)
  {
    return _bound.Contains(x, y);
  }


  public bool IsSvgElementVisible()
  {
    return _svgElement.IsVisible();
  }


  private RectangleF GetSvgElementBound()
  {
    var bound = _svgElement.Bounds;

    if (_svgElement.Parent is SvgGroup parentGroup)
    {
      parentGroup.Transforms?.ForEach(t =>
      {
        if (t is SvgTranslate translate)
        {
          bound.X += translate.X;
          bound.Y += translate.Y;
        }
      });
    }
    bound.X      *= _owner.ViewBoxWidthCoeff;
    bound.Y      *= _owner.ViewBoxHeightCoeff;
    bound.Width  *= _owner.ViewBoxWidthCoeff;
    bound.Height *= _owner.ViewBoxHeightCoeff;
    
    return bound;
  }


  private void SetElementProperty(string id, string property, string value)
  {
    if (!_elementsWithId.TryGetValue(id, out var element))
    {
      LogMessage($"Unknown element {id}");
      return;
    }
    try
    {
      element.SetProperty(property, value);
    }
    catch (Exception ex)
    {
      LogMessage(ex.Message);
    }
  }


  private string GetElementProperty(string id, string property)
  {
    if (!_elementsWithId.TryGetValue(id, out var element))
    {
      LogMessage($"Unknown element {id}");
      return null;
    }
    try
    {
      return element.GetProperty(property);
    }
    catch (Exception ex)
    {
      LogMessage(ex.Message);
      return null;
    }
  }


  private void LogMessage(string message)
  {
    _owner.AddToLog(message, this);
  }


  public sealed override string ToString()
  {
    return $"{_svgElement.ID}   {(_script.Length <= 80 ? _script : $"{_script[..80]}…")}";
  }


  private void InitTmStatusDefaultCommands(int parameter)
  {
    InitClickCommand((int)ElementCommandType.ShowTmStatus, parameter);

    InitContextMenuCommand((int)ElementCommandType.Telecontrol,            parameter);
    InitContextMenuCommand((int)ElementCommandType.SwitchTmStatusManually, parameter);
    InitContextMenuCommand((int)ElementCommandType.AckTmStatus,            parameter);
    InitContextMenuSeparator();
    InitContextMenuCommand((int)ElementCommandType.OpenTmStatusEventsArchive, parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmStatusChart,         parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmStatusReport,        parameter);
    InitContextMenuSeparator();
    InitContextMenuCommand((int)ElementCommandType.CopyTmStatusNameToClipboard, parameter);
    InitContextMenuCommand((int)ElementCommandType.CopyTmStatusToClipboard,     parameter);
    InitContextMenuCommand((int)ElementCommandType.AddTmStatusToQuickList,      parameter);
  }


  private void InitTmAnalogDefaultCommands(int parameter)
  {
    InitClickCommand((int)ElementCommandType.ShowTmAnalog, parameter);

    InitContextMenuCommand((int)ElementCommandType.Teleregulation,      parameter);
    InitContextMenuCommand((int)ElementCommandType.SetTmAnalogManually, parameter);
    InitContextMenuSeparator();
    InitContextMenuCommand((int)ElementCommandType.OpenTmAnalogTechProperties, parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmAnalogAlarms,         parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmAnalogEventsArchive,  parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmAnalogChart,          parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenTmAnalogReport,         parameter);
    InitContextMenuSeparator();
    InitContextMenuCommand((int)ElementCommandType.CopyTmAnalogNameToClipboard, parameter);
    InitContextMenuCommand((int)ElementCommandType.CopyTmAnalogToClipboard,     parameter);
    InitContextMenuCommand((int)ElementCommandType.AddTmAnalogToQuickList,      parameter);
  }


  private void InitOpenDocumentDefaultCommands(string parameter)
  {
    InitClickCommand((int)ElementCommandType.OpenDocumentInThisTab, parameter);

    InitContextMenuCommand((int)ElementCommandType.OpenDocumentInThisTab,   parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenDocumentInNewTab,    parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenDocumentInUniqueTab, parameter);
    InitContextMenuCommand((int)ElementCommandType.OpenDocumentInOverview,  parameter);
  }


  private void InitCustomToolTip(string text)
  {
    CustomToolTip = text;
  }


  private void InitClickCommand(int type, int parameter)
  {
    if (!Enum.IsDefined(typeof(ElementCommandType), type))
    {
      return;
    }
    ClickCommand = new ElementCommand(ElementCommandLevel.Click,
                                      (ElementCommandType)type,
                                      parameter);
  }


  private void InitClickCommand(int type, string parameter)
  {
    if (!Enum.IsDefined(typeof(ElementCommandType), type))
    {
      return;
    }
    ClickCommand = new ElementCommand(ElementCommandLevel.Click,
                                      (ElementCommandType)type,
                                      parameter);
  }


  private void InitContextMenuSeparator()
  {
    ContextMenuCommands.Add(new ElementCommand(ElementCommandLevel.ContextMenu,
                                               ElementCommandType.None,
                                               null));
  }


  private void InitContextMenuCommand(int type, int parameter, string caption = null)
  {
    if (!Enum.IsDefined(typeof(ElementCommandType), type))
    {
      return;
    }
    ContextMenuCommands.Add(new ElementCommand(ElementCommandLevel.ContextMenu,
                                               (ElementCommandType)type,
                                               parameter,
                                               caption));
  }


  private void InitContextMenuCommand(int type, string parameter, string caption = null)
  {
    if (!Enum.IsDefined(typeof(ElementCommandType), type))
    {
      return;
    }
    ContextMenuCommands.Add(new ElementCommand(ElementCommandLevel.ContextMenu,
                                               (ElementCommandType)type,
                                               parameter,
                                               caption));
  }
}