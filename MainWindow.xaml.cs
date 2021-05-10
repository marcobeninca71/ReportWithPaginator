using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;


namespace WpfAppReports
{
    public static class Extensions
    {
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
               bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double dpi = 96;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private DelegateCommand _printCommand = null;
        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                    _printCommand = new DelegateCommand(new Action<object>(Print));
                return _printCommand;
            }
        }


        private DelegateCommand _printDirectQueueCommand = null;
        public ICommand PrintDirectQueueCommand
        {
            get
            {
                if (_printDirectQueueCommand == null)
                    _printDirectQueueCommand = new DelegateCommand(new Action<object>(PrintDirectQueue));
                return _printDirectQueueCommand;
            }
        }

        private void Print(object obj)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                ImageSource s1 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/Resources/2.jpg");
                ImageSource s2 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/Resources/box.jpg");
                ImageSource s4 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/Resources/2.jpg");
                ImageSource s3 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                        "pack://application:,,,/Resources/box.jpg");
                var paginator = new MyPaginator() { rows = new List<Model>() { 
                        new Model() { Testo1="1", Testo2="2", Image1Source= s1, Image2Source=s2 }, 
                        new Model() { Testo1 = "3", Testo2 = "4", Image1Source=s3, Image2Source=s4 }
                    }, 
                    PageSize=new Size(450, 800) 
                };
                dialog.PrintDocument(paginator, "Rows Document");
            }
        }

        private void PrintDirectQueue(object obj)
        {
            ImageSource s1 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                    "pack://application:,,,/Resources/2.jpg");
            ImageSource s2 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                    "pack://application:,,,/Resources/box.jpg");
            ImageSource s4 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                    "pack://application:,,,/Resources/2.jpg");
            ImageSource s3 = (ImageSource)new ImageSourceConverter().ConvertFromString(
                    "pack://application:,,,/Resources/box.jpg");
            var paginator = new MyPaginator()
            {
                rows = new List<Model>() {
                    new Model() { Testo1="1", Testo2="2", Image1Source= s1, Image2Source=s2 },
                    new Model() { Testo1 = "3", Testo2 = "4", Image1Source=s3, Image2Source=s4 }
                },
                PageSize = new Size(4.015 * dpi, 2.008 * dpi)
            };

            LocalPrintServer server = new LocalPrintServer();
            PrintQueue queue = server.GetPrintQueue("Microsoft Print to PDF");

            queue.CurrentJobSettings.CurrentPrintTicket.PageMediaSize = new PageMediaSize(4.015 * dpi, 2.008 * dpi);
            queue.Commit();
            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);
            writer.Write(paginator);
        }

    }
}
