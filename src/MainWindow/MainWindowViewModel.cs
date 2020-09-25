using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Iface.Oik.SvgPlayground.Model;
using Iface.Oik.SvgPlayground.Util;
using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Svg;

namespace Iface.Oik.SvgPlayground.MainWindow
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private const float ScaleStep = 1.5f;
    private const float MoveStep  = 50f;

    private readonly MainWindowView _view;

    private          string        _svgFilename;
    private          SvgDocument   _svgDocument;
    private readonly List<Element> _elements = new List<Element>();

    private string _title;

    private float _scale = 1;
    private float _x     = 0;
    private float _y     = 0;


    public string Title
    {
      get => _title;
      set
      {
        _title = value;
        NotifyOnPropertyChanged();
      }
    }


    public ObservableCollection<TmStatus> TmStatuses { get; } = new ObservableCollection<TmStatus>();
    public ObservableCollection<TmAnalog> TmAnalogs  { get; } = new ObservableCollection<TmAnalog>();
    public ObservableCollection<Variable> Variables  { get; } = new ObservableCollection<Variable>();


    public ICommand OpenFileCommand   { get; }
    public ICommand ReloadFileCommand { get; }
    public ICommand ZoomInCommand     { get; }
    public ICommand ZoomOutCommand    { get; }
    public ICommand Zoom1Command      { get; }
    public ICommand MoveUpCommand     { get; }
    public ICommand MoveRightCommand  { get; }
    public ICommand MoveDownCommand   { get; }
    public ICommand MoveLeftCommand   { get; }


    public MainWindowViewModel(MainWindowView view)
    {
      _view = view;

      Title = "SVG";

      OpenFileCommand   = new RelayCommand(_ => OpenFile());
      ReloadFileCommand = new RelayCommand(_ => ReloadFile());
      ZoomInCommand     = new RelayCommand(_ => ZoomIn());
      ZoomOutCommand    = new RelayCommand(_ => ZoomOut());
      Zoom1Command      = new RelayCommand(_ => Zoom1());
      MoveUpCommand     = new RelayCommand(_ => MoveUp());
      MoveRightCommand  = new RelayCommand(_ => MoveRight());
      MoveDownCommand   = new RelayCommand(_ => MoveDown());
      MoveLeftCommand   = new RelayCommand(_ => MoveLeft());

      FixTextBoxFloatValue();
    }


    private static void FixTextBoxFloatValue()
    {
      FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
    }


    private void OpenFile()
    {
      var openFileDialog = new OpenFileDialog
      {
        Filter = "SVG (*.svg)|*.svg",
      };
      if (openFileDialog.ShowDialog() != true)
      {
        return;
      }

      OpenFile(openFileDialog.FileName);
    }


    private void ReloadFile()
    {
      OpenFile(_svgFilename);
    }


    private void OpenFile(string filename)
    {
      if (filename == null) return;

      _svgFilename = filename;
      try
      {
        ClearEverything();
        _svgDocument = SvgDocument.Open(filename);
        Title        = SvgUtil.FindTitleElement(_svgDocument)?.Content ?? "SVG";
        ParseElements();
        Update();
      }
      catch (Exception ex)
      {
        _svgDocument = null;
        MessageBox.Show("Ошибка при открытии файла: " + ex.Message);
      }
    }


    private void ClearEverything()
    {
      TmStatuses.Clear();
      TmAnalogs.Clear();
      _elements.Clear();
    }


    private void ParseElements()
    {
      _elements.Clear();

      try
      {
        var svgElements = SvgUtil.GetElementsCollectionWithAttribute(_svgDocument, "oikelement");
        foreach (var svgElement in svgElements)
        {
          var element = Element.Create(this, svgElement);
          if (element != null)
          {
            _elements.Add(element);
          }
        }
      }
      catch (Exception ex)
      {
        _elements.Clear();
        MessageBox.Show("Ошибка при разборе SVG, схема не будет оживлена: " + ex.Message);
      }
    }


    private void Update()
    {
      if (_svgDocument == null) return;

      foreach (var element in _elements)
      {
        element.Update();
      }

      InvalidateCanvas();
    }


    private void InvalidateCanvas()
    {
      Application.Current
                 .Dispatcher
                 ?.Invoke(() => _view?.InvalidateCanvas());
    }


    public void OnCanvasPaintSurface(SKCanvas canvas)
    {
      try
      {
        SkiaSvgUtil.PaintSvgDocumentToSkiaCanvas(_svgDocument, canvas, _x, _y, _scale);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Ошибка при попытке отрисовки: " + ex.Message);
      }
    }


    private void ZoomIn()
    {
      Zoom(ScaleStep);
    }


    private void ZoomOut()
    {
      Zoom(1 / ScaleStep);
    }


    private void Zoom1()
    {
      Zoom(1 / _scale);
    }


    private void Zoom(float scale)
    {
      var tempScale = _scale * scale;
      if (tempScale < 1.1f && tempScale > 0.9f && !tempScale.Equals(1.0f)) // округляем до единицы, если рядом
      {
        scale /= tempScale;
      }

      _scale *= scale;

      Update();
    }


    private void MoveUp()
    {
      Move(0, -MoveStep);
    }


    private void MoveRight()
    {
      Move(MoveStep, 0);
    }


    private void MoveDown()
    {
      Move(0, +MoveStep);
    }


    private void MoveLeft()
    {
      Move(-MoveStep, 0);
    }


    private void Move(float x, float y)
    {
      _x += x;
      _y += y;
      
      Update();
    }


    private int FindTmStatus(int ch, int rtu, int point)
    {
      var index = 0;
      foreach (var tmStatus in TmStatuses)
      {
        if (tmStatus.AddrEquals(ch, rtu, point))
        {
          return index;
        }
        index++;
      }
      return -1;
    }


    private int FindTmAnalog(int ch, int rtu, int point)
    {
      var index = 0;
      foreach (var tmAnalog in TmAnalogs)
      {
        if (tmAnalog.AddrEquals(ch, rtu, point))
        {
          return index;
        }
        index++;
      }
      return -1;
    }


    private int FindVariable(string id)
    {
      var index = 0;
      foreach (var variable in Variables)
      {
        if (variable.Id == id)
        {
          return index;
        }
        index++;
      }
      return -1;
    }


    public int InitVariable(string id, string _ = null)
    {
      var existingVariable = FindVariable(id);
      if (existingVariable >= 0)
      {
        return existingVariable;
      }

      var variable = new Variable(id);
      Variables.Add(variable);

      return Variables.Count - 1;
    }


    public int InitTmStatus(int ch, int rtu, int point, string _ = null)
    {
      var existingTmStatusIndex = FindTmStatus(ch, rtu, point);
      if (existingTmStatusIndex >= 0)
      {
        return existingTmStatusIndex;
      }

      var tmStatus = new TmStatus(ch, rtu, point);

      TmStatuses.Add(tmStatus);

      tmStatus.PropertyChanged += (s, ea) => Update();

      return TmStatuses.Count - 1;
    }


    public int InitTmAnalog(int ch, int rtu, int point, string _ = null)
    {
      var existingTmAnalogIndex = FindTmAnalog(ch, rtu, point);
      if (existingTmAnalogIndex >= 0)
      {
        return existingTmAnalogIndex;
      }

      var tmAnalog = new TmAnalog(ch, rtu, point);

      TmAnalogs.Add(tmAnalog);

      tmAnalog.PropertyChanged += (s, ea) => Update();

      return TmAnalogs.Count - 1;
    }


    public int GetTmStatusStatus(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return -1;
      }
      return tmStatus.IsOn ? 1 : 0;
    }


    public string GetTmStatusCaption(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return "???";
      }
      return tmStatus.StatusCaption;
    }


    public bool IsTmStatusOn(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsOn;
    }


    public bool IsTmStatusUnreliable(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsUnreliable;
    }


    public bool IsTmStatusMalfunction(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsMalfunction;
    }


    public bool IsTmStatusIntermediate(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsIntermediate;
    }


    public bool IsTmStatusManuallySet(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsManuallySet;
    }


    public bool IsTmStatusUnderMaintenance(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsUnderMaintenance;
    }


    public bool IsTmStatusUnderCheckup(int idx)
    {
      var tmStatus = TmStatuses.ElementAtOrDefault(idx);
      if (tmStatus == null)
      {
        return false;
      }
      return tmStatus.IsUnderCheckup;
    }


    public bool IsTmAnalogUnreliable(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return false;
      }
      return tmAnalog.IsUnreliable;
    }


    public bool IsTmAnalogAlarmAlert(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return false;
      }
      return tmAnalog.IsAlarmLevel4;
    }


    public bool IsTmAnalogAlarmWarning(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return false;
      }
      return tmAnalog.IsAlarmLevel3;
    }


    public float GetTmAnalogValue(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return 0;
      }
      return tmAnalog.Value;
    }


    public string GetTmAnalogValueString(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return TmAnalog.InvalidValueString;
      }
      return tmAnalog.ValueString;
    }


    public string GetTmAnalogUnit(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return string.Empty;
      }
      return tmAnalog.Unit;
    }


    public string GetTmAnalogValueWithUnitString(int idx)
    {
      var tmAnalog = TmAnalogs.ElementAtOrDefault(idx);
      if (tmAnalog == null)
      {
        return TmAnalog.InvalidValueString;
      }
      return tmAnalog.ValueWithUnitString;
    }


    public bool IsVariableUnreliable(int idx)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return false;
      }
      return variable.IsUnreliable;
    }


    public bool IsVariableManuallySet(int idx)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return false;
      }
      return variable.IsManuallySet;
    }


    public bool IsVariableOn(int idx)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return false;
      }
      return variable.IsOn;
    }


    public void SetVariable(int idx, bool isOn)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return;
      }
      variable.IsOn = isOn;
    }


    public void SetVariableUnreliable(int idx, bool isUnreliable)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return;
      }
      variable.IsUnreliable = isUnreliable;
    }


    public void SetVariableManuallySet(int idx, bool isManuallySet)
    {
      var variable = Variables.ElementAtOrDefault(idx);
      if (variable == null)
      {
        return;
      }
      variable.IsManuallySet = isManuallySet;
    }


    public event PropertyChangedEventHandler PropertyChanged;


    private void NotifyOnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}