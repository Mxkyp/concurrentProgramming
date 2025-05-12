//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable, IDataErrorInfo
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { 
    }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
    }


    #endregion ctor

    #region public API

    public void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      ModelLayer.Start(numberOfBalls, tableWidth, tableHeight, tableBorder);
      Observer.Dispose();
    }

    public void initBindings(double screenWidth, double screenHeight) { 
      this.WindowWidth = screenWidth / 1.5;
      this.WindowHeight = screenHeight / 1.3;
      this.TableWidth = screenWidth / 3;
      this.TableHeight = screenHeight / 2;
      this.TableBorder = 6;
    }

    public RelayCommand ReadTextBox => new RelayCommand(() => readTextBox());

    public String NumberOfBalls
    {
        get => _numberOfBalls;
        set
        {
            if (_numberOfBalls != value)
            {
                _numberOfBalls = value;
                RaisePropertyChanged(nameof(Error));
            }
        }
    }
    public double TableHeight 
    {
        get => tableHeight;
        set 
        {
            if (tableHeight != value)
            {
                tableHeight = value;
                RaisePropertyChanged();
            }
        }
    }

    public double TableWidth
    {
        get => tableWidth;
        set 
        {
            if (tableWidth != value)
            {
                tableWidth = value;
                RaisePropertyChanged();
            }
        }
    }
    public double WindowHeight 
    {
        get => windowHeight;
        set 
        {
            if (windowHeight != value)
            {
                windowHeight = value;
                RaisePropertyChanged();
            }
        }
    }

    public double WindowWidth 
    {
        get => windowWidth;
        set 
        {
            if (windowWidth != value)
            {
                windowWidth = value;
                RaisePropertyChanged();
            }
        }
    }
    public double TableBorder 
    {
        get => tableBorder;
        set 
        {
            if (tableBorder != value)
            {
                tableBorder = value;
                RaisePropertyChanged();
            }
        }
    }

    public bool InputEnabled 
    {
        get => inputEnabled;
        set
        {
            if (inputEnabled != value)
            {
                inputEnabled = value;
            }
        }
    }
    public string this[string columnName]
    {
        get
        {
            if (columnName == nameof(NumberOfBalls))
            {
                if (!int.TryParse(NumberOfBalls, out int validNumber))
                {
                    return "Invalid number of balls.";
                }
                else if (validNumber < 1 || validNumber > 20)
                {
                    return "Number of balls must be between 1 and 20.";
                }
            }
            return "";
        }
    } 

    public string Error
    {
        get
        {
            string error = this[nameof(NumberOfBalls)];
            return error;
        }
    }


    public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();



        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;
    private String _numberOfBalls = "5";

    private double windowWidth;
    private double windowHeight;
    private double tableWidth;
    private double tableHeight;
    private double tableBorder;

    private bool inputEnabled = true;
    private string error = "";
        private void readTextBox()
    {
        if (int.TryParse(_numberOfBalls, out int validNumber))
        {
            RaisePropertyChanged();
            if (validNumber > 0 && validNumber < 21 )
            {
                this.Start(validNumber);
                inputEnabled = false;
                RaisePropertyChanged(nameof(InputEnabled));
            }
        }
    }

    #endregion private
  }
}