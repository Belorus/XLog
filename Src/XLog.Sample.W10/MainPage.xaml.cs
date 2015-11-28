using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace XLog.Sample.W10
{
    public sealed partial class MainPage
    {
        class FinalizeableClass
        {
            ~FinalizeableClass()
            {
                throw new Exception("Exception in finalizer");
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ThroExceptionOnMainThread(object sender, RoutedEventArgs e)
        {
            throw new Exception("Main thread exc");
        }

        private void ThrowUnobservedTaskException(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                throw new Exception("Unobserved task exc");
            });

            Task.Delay(50).Wait();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void ThrowExceptionOnFinalizerThread(object sender, RoutedEventArgs e)
        {
            new FinalizeableClass();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private async void ThrowAsyncVoidException(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);

            throw new Exception("Async void exception");
        }
    }
}
