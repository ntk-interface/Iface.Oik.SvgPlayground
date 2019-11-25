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

      _scriptEngine
        .SetValue("setElementProperty",            new Action<string, string, string>(SetElementProperty))
        .SetValue("getElementProperty",            new Func<string, string, string>(GetElementProperty))
        .SetValue("initTmStatus",                  new Func<int, int, int, string, int>(_owner.InitTmStatus))
        .SetValue("initTmAnalog",                  new Func<int, int, int, string, int>(_owner.InitTmAnalog))
        .SetValue("initVariable",                  new Func<string, string, int>(_owner.InitVariable))
        .SetValue("getTmStatusStatus",             new Func<int, int>(_owner.GetTmStatusStatus))
        .SetValue("isTmStatusUnreliable",          new Func<int, bool>(_owner.IsTmStatusUnreliable))
        .SetValue("isTmStatusMalfunction",         new Func<int, bool>(_owner.IsTmStatusMalfunction))
        .SetValue("isTmStatusIntermediate",        new Func<int, bool>(_owner.IsTmStatusIntermediate))
        .SetValue("isTmStatusOn",                  new Func<int, bool>(_owner.IsTmStatusOn))
        .SetValue("getTmAnalogValue",              new Func<int, string>(_owner.GetTmAnalogValue))
        .SetValue("isTmAnalogUnreliable",          new Func<int, bool>(_owner.IsTmAnalogUnreliable))
        .SetValue("isVariableUnreliable",          new Func<int, bool>(_owner.IsVariableUnreliable))
        .SetValue("isVariableOn",                  new Func<int, bool>(_owner.IsVariableOn))
        .SetValue("setVariable",                   new Action<int, bool?>(_owner.SetVariable))
        .SetValue("initTmStatusDefaultCommands",   new Action<int>(InitTmStatusDefaultCommands))
        .SetValue("initTmAnalogDefaultCommands",   new Action<int>(InitTmAnalogDefaultCommands))
        .SetValue("initOpenSvgDefaultCommands",    new Action<string>(InitOpenSvgDefaultCommands))
        .SetValue("initClickIntCommand",           new Action<int, int>(InitClickCommand))
        .SetValue("initContextMenuIntCommand",     new Action<int, int, string>(InitContextMenuCommand))
        .SetValue("initClickStringCommand",        new Action<int, string>(InitClickCommand))
        .SetValue("initContextMenuStringCommand",  new Action<int, string, string>(InitContextMenuCommand))
        .SetValue("initEmptyContextMenuCommand",   new Action(InitEmptyContextMenuCommand))
        .SetValue("NONE",                          ElementCommandType.None)
        .SetValue("OPEN_SVG_IN_THIS_TAB",          ElementCommandType.OpenSvgInThisTab)
        .SetValue("OPEN_SVG_IN_NEW_TAB",           ElementCommandType.OpenSvgInNewTab)
        .SetValue("SHOW_TM_STATUS",                ElementCommandType.ShowTmStatus)
        .SetValue("TELECONTROL",                   ElementCommandType.Telecontrol)
        .SetValue("SWITCH_TM_STATUS_MANUALLY",     ElementCommandType.SwitchTmStatusManually)
        .SetValue("ACK_TM_STATUS",                 ElementCommandType.AckTmStatus)
        .SetValue("OPEN_TM_STATUS_EVENTS_ARCHIVE", ElementCommandType.OpenTmStatusEventsArchive)
        .SetValue("COPY_TM_STATUS_TO_CLIPBOARD",   ElementCommandType.CopyTmStatusToClipboard)
        .SetValue("SHOW_TM_ANALOG",                ElementCommandType.ShowTmAnalog)
        .SetValue("SET_TM_ANALOG_MANUALLY",        ElementCommandType.SetTmAnalogManually)
        .SetValue("OPEN_TM_ANALOG_ALARMS",         ElementCommandType.OpenTmAnalogAlarms)
        .SetValue("OPEN_TM_ANALOG_CHART",          ElementCommandType.OpenTmAnalogChart)
        .SetValue("OPEN_TM_ANALOG_EVENTS_ARCHIVE", ElementCommandType.OpenTmAnalogEventsArchive)
        .SetValue("COPY_TM_ANALOG_TO_CLIPBOARD",   ElementCommandType.CopyTmAnalogToClipboard)
        .Execute(_script, new ParserOptions {Tolerant = true});

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

      SvgUtil.SetElementProperty(element, property, value);
    }


    private string GetElementProperty(string id, string property)
    {
      if (!_elementsWithId.TryGetValue(id, out var element)) return null;

      return SvgUtil.GetElementProperty(element, property);
    }


    // эти команды никак не будут использованы в эмуляторе, описаны для совместимости
    private void InitTmStatusDefaultCommands(int parameter)
    {
    }


    private void InitTmAnalogDefaultCommands(int parameter)
    {
    }


    private void InitOpenSvgDefaultCommands(string parameter)
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