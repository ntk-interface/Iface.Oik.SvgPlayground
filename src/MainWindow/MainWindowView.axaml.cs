using Avalonia.Controls;
using Avalonia.Input;
using Iface.Oik.SvgPlayground.Components;

namespace Iface.Oik.SvgPlayground.MainWindow;

public partial class MainWindowView : Window
{
  private readonly MainWindowViewModel _viewModel;


  public MainWindowView()
  {
    InitializeComponent();

    _viewModel = new MainWindowViewModel(this);

    DataContext = _viewModel;
  }


  public void InvalidateCanvas()
  {
    Canvas.InvalidateSurface();
  }


  private void OnPaintSurface(object _, SKPaintSurfaceEventArgs e)
  {
    _viewModel?.OnCanvasPaintSurface(e.Surface.Canvas);
  }


  private void OnPointerReleased(object _, PointerReleasedEventArgs e)
  {
    var position = e.GetPosition(Canvas);

    _viewModel?.OnClick((float)position.X, (float)position.Y);
  }
}