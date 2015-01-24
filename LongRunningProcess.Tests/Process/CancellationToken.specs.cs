﻿using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LongRunningProcess.Tests.Process
{
    [Subject(typeof(LongRunningProcess.Process))]
    public class When_getting_cancellation_token
    {
        Establish context = () =>
        {
            TokenSource = new CancellationTokenSource();

            Process = new LongRunningProcess.Process(string.Empty, null, TokenSource);

            Token = Process.CancellationToken;
        };

        Because of = () => TokenSource.Cancel();

        It Should_cancel_from_the_given_source = () => Token.IsCancellationRequested.ShouldBeTrue();

        static IProcess Process;

        static CancellationTokenSource TokenSource;

        static CancellationToken Token;
    }
}