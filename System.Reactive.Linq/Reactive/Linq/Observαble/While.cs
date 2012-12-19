﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

#if !NO_PERF
using System;
using System.Collections.Generic;

namespace System.Reactive.Linq.Observαble
{
    class While<TSource> : Producer<TSource>, IConcatenatable<TSource>
    {
        private readonly Func<bool> _condition;
        private readonly IObservable<TSource> _source;

        public While(Func<bool> condition, IObservable<TSource> source)
        {
            _condition = condition;
            _source = source;
        }

        protected override IDisposable Run(IObserver<TSource> observer, IDisposable cancel, Action<IDisposable> setSink)
        {
            var sink = new _(observer, cancel);
            setSink(sink);
            return sink.Run(GetSources());
        }

        public IEnumerable<IObservable<TSource>> GetSources()
        {
            while (_condition())
                yield return _source;
        }

        class _ : ConcatSink<TSource>
        {
            public _(IObserver<TSource> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(TSource value)
            {
                base._observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                base._observer.OnError(error);
                base.Dispose();
            }
        }
    }
}
#endif
