﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

namespace System.Reactive.Linq.ObservableImpl
{
    internal sealed class Cast<TSource, TResult> : Producer<TResult, Cast<TSource, TResult>._> /* Could optimize further by deriving from Select<TResult> and providing Combine<TResult2>. We're not doing this (yet) for debuggability. */
    {
        private readonly IObservable<TSource> _source;

        public Cast(IObservable<TSource> source)
        {
            _source = source;
        }

        protected override _ CreateSink(IObserver<TResult> observer, IDisposable cancel) => new _(observer, cancel);

        protected override IDisposable Run(_ sink) => _source.SubscribeSafe(sink);

        internal sealed class _ : Sink<TSource, TResult> 
        {
            public _(IObserver<TResult> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(TSource value)
            {
                var result = default(TResult);
                try
                {
                    result = (TResult)(object)value;
                }
                catch (Exception exception)
                {
                    base.ForwardOnError(exception);
                    return;
                }

                base.ForwardOnNext(result);
            }
        }
    }
}
