using System;
using System.Collections.Generic;
using Iface.Oik.SvgPlayground.MainWindow;
using Iface.Oik.SvgPlayground.Util;
using Jint;
using Jint.Parser;
using Svg;

namespace Iface.Oik.SvgPlayground.Model
{
  public class Element
  {
    private readonly MainWindowViewModel _owner;
    private readonly SvgVisualElement    _svgElement;

    private readonly string                          _script;
    private readonly Engine                          _scriptEngine;
    private readonly IDictionary<string, SvgElement> _elementsWithId;
    private readonly bool                            _isUpdatable;

    public bool IsInit { get; }


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
          .SetValue("logMessage",                      new Action<string>(LogMessage))
          .SetValue("setElementProperty",              new Action<string, string, string>(SetElementProperty))
          .SetValue("getElementProperty",              new Func<string, string, string>(GetElementProperty))
          .SetValue("initTmStatus",                    new Func<int, int, int, string, int>(_owner.InitTmStatus))
          .SetValue("initTmAnalog",                    new Func<int, int, int, string, int>(_owner.InitTmAnalog))
          .SetValue("initVariable",                    new Func<string, string, int>(_owner.InitVariable))
          .SetValue("getTmStatusStatus",               new Func<int, int>(_owner.GetTmStatusStatus))
          .SetValue("getTmStatusCaption",              new Func<int, string>(_owner.GetTmStatusCaption))
          .SetValue("isTmStatusUnreliable",            new Func<int, bool>(_owner.IsTmStatusUnreliable))
          .SetValue("isTmStatusMalfunction",           new Func<int, bool>(_owner.IsTmStatusMalfunction))
          .SetValue("isTmStatusIntermediate",          new Func<int, bool>(_owner.IsTmStatusIntermediate))
          .SetValue("isTmStatusManuallySet",           new Func<int, bool>(_owner.IsTmStatusManuallySet))
          .SetValue("isTmStatusManuallyBlocked",       new Func<int, bool>(_owner.IsTmStatusManuallyBlocked))
          .SetValue("isTmStatusAbnormal",              new Func<int, bool>(_owner.IsTmStatusAbnormal))
          .SetValue("isTmStatusUnacked",               new Func<int, bool>(_owner.IsTmStatusUnacked))
          .SetValue("isTmStatusCustomFlagRaised",      new Func<int, int, bool>(_owner.IsTmStatusCustomFlagRaised))
          .SetValue("isTmStatusUnderMaintenance",      new Func<int, bool>(_owner.IsTmStatusUnderMaintenance))
          .SetValue("isTmStatusUnderCheckup",          new Func<int, bool>(_owner.IsTmStatusUnderCheckup))
          .SetValue("isTmStatusOn",                    new Func<int, bool>(_owner.IsTmStatusOn))
          .SetValue("getTmAnalogValue",                new Func<int, float>(_owner.GetTmAnalogValue))
          .SetValue("getTmAnalogUnit",                 new Func<int, string>(_owner.GetTmAnalogUnit))
          .SetValue("getTmAnalogValueString",          new Func<int, string>(_owner.GetTmAnalogValueString))
          .SetValue("getTmAnalogValueWithUnitString",  new Func<int, string>(_owner.GetTmAnalogValueWithUnitString))
          .SetValue("isTmAnalogUnreliable",            new Func<int, bool>(_owner.IsTmAnalogUnreliable))
          .SetValue("isTmAnalogManuallySet",           new Func<int, bool>(_owner.IsTmAnalogManuallySet))
          .SetValue("isTmAnalogManuallyBlocked",       new Func<int, bool>(_owner.IsTmAnalogManuallyBlocked))
          .SetValue("isTmAnalogAbnormal",              new Func<int, bool>(_owner.IsTmAnalogAbnormal))
          .SetValue("isTmAnalogUnacked",               new Func<int, bool>(_owner.IsTmAnalogUnacked))
          .SetValue("isTmAnalogAlarmAlert",            new Func<int, bool>(_owner.IsTmAnalogAlarmAlert))
          .SetValue("isTmAnalogAlarmWarning",          new Func<int, bool>(_owner.IsTmAnalogAlarmWarning))
          .SetValue("isTmAnalogAlarmFlagRaised",       new Func<int, int, bool>(_owner.IsTmAnalogAlarmFlagRaised))
          .SetValue("isVariableUnreliable",            new Func<int, bool>(_owner.IsVariableUnreliable))
          .SetValue("isVariableManuallySet",           new Func<int, bool>(_owner.IsVariableManuallySet))
          .SetValue("isVariableOn",                    new Func<int, bool>(_owner.IsVariableOn))
          .SetValue("setVariable",                     new Action<int, bool>(_owner.SetVariable))
          .SetValue("setVariableUnreliable",           new Action<int, bool>(_owner.SetVariableUnreliable))
          .SetValue("setVariableManuallySet",          new Action<int, bool>(_owner.SetVariableManuallySet))
          .SetValue("initTmStatusDefaultCommands",     new Action<int>(InitTmStatusDefaultCommands))
          .SetValue("initTmAnalogDefaultCommands",     new Action<int>(InitTmAnalogDefaultCommands))
          .SetValue("initOpenDocumentDefaultCommands", new Action<string>(InitOpenDocumentDefaultCommands))
          .SetValue("initClickIntCommand",             new Action<int, int>(InitClickCommand))
          .SetValue("initContextMenuIntCommand",       new Action<int, int, string>(InitContextMenuCommand))
          .SetValue("initClickStringCommand",          new Action<int, string>(InitClickCommand))
          .SetValue("initContextMenuStringCommand",    new Action<int, string, string>(InitContextMenuCommand))
          .SetValue("initEmptyContextMenuCommand",     new Action(InitEmptyContextMenuCommand))
          .SetValue("NONE",                            ElementCommandType.None)
          .SetValue("OPEN_DOCUMENT_IN_THIS_TAB",       ElementCommandType.OpenDocumentInThisTab)
          .SetValue("OPEN_SVG_IN_NEW_TAB",             ElementCommandType.OpenDocumentInNewTab)
          .SetValue("SHOW_TM_STATUS",                  ElementCommandType.ShowTmStatus)
          .SetValue("TELECONTROL",                     ElementCommandType.Telecontrol)
          .SetValue("SWITCH_TM_STATUS_MANUALLY",       ElementCommandType.SwitchTmStatusManually)
          .SetValue("ACK_TM_STATUS",                   ElementCommandType.AckTmStatus)
          .SetValue("OPEN_TM_STATUS_EVENTS_ARCHIVE",   ElementCommandType.OpenTmStatusEventsArchive)
          .SetValue("COPY_TM_STATUS_TO_CLIPBOARD",     ElementCommandType.CopyTmStatusToClipboard)
          .SetValue("SHOW_TM_ANALOG",                  ElementCommandType.ShowTmAnalog)
          .SetValue("TELEREGULATE",                    ElementCommandType.Teleregulation)
          .SetValue("SET_TM_ANALOG_MANUALLY",          ElementCommandType.SetTmAnalogManually)
          .SetValue("OPEN_TM_ANALOG_TECH_PROPERTIES",  ElementCommandType.OpenTmAnalogTechProperties)
          .SetValue("OPEN_TM_ANALOG_ALARMS",           ElementCommandType.OpenTmAnalogAlarms)
          .SetValue("OPEN_TM_ANALOG_CHART",            ElementCommandType.OpenTmAnalogChart)
          .SetValue("OPEN_TM_ANALOG_EVENTS_ARCHIVE",   ElementCommandType.OpenTmAnalogEventsArchive)
          .SetValue("COPY_TM_ANALOG_TO_CLIPBOARD",     ElementCommandType.CopyTmAnalogToClipboard)
          .Execute(_script, new ParserOptions { Tolerant = true });
      }
      catch (Exception ex)
      {
        throw new Exception(ToString() + Environment.NewLine + ex.Message);
      }

      _isUpdatable = _scriptEngine.Global.HasProperty("update");

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

      _scriptEngine.Invoke("update");
    }


    private void SetElementProperty(string id, string property, string value)
    {
      if (!_elementsWithId.TryGetValue(id, out var element)) return;

      element?.SetProperty(property, value);
    }


    private string GetElementProperty(string id, string property)
    {
      if (!_elementsWithId.TryGetValue(id, out var element)) return null;

      return element?.GetProperty(property);
    }


    private void LogMessage(string message)
    {
      Console.WriteLine($"{message} ({ToString()})");
    }


    public sealed override string ToString()
    {
      var scriptTruncatedText = _script.Length < 100 ? _script : _script[..100] + "…";
      return _svgElement.ID + Environment.NewLine + scriptTruncatedText;
    }


    // эти команды никак не будут использованы в эмуляторе, описаны для совместимости
    private void InitTmStatusDefaultCommands(int parameter)
    {
    }


    private void InitTmAnalogDefaultCommands(int parameter)
    {
    }


    private void InitOpenDocumentDefaultCommands(string parameter)
    {
    }


    private void InitClickCommand(int type, int parameter)
    {
    }


    private void InitClickCommand(int type, string parameter)
    {
    }


    private void InitEmptyContextMenuCommand()
    {
    }


    private void InitContextMenuCommand(int type, int parameter, string caption = null)
    {
    }


    private void InitContextMenuCommand(int type, string parameter, string caption = null)
    {
    }
  }
}