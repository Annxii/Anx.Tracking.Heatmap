using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Anx.Tracking.Heatmap
{
    public static class IObservableExtensions
    {
        public static void SubscribeDisposable<T>(this IObservable<T> obs, CompositeDisposable disp)
        {
            var x = obs.Subscribe();
            disp.Add(x);
        }

        public static void ConnectDisposable<T>(this IConnectableObservable<T> obs, CompositeDisposable disp)
        {
            var x = obs.Connect();
            disp.Add(x);
        }
    }
}
