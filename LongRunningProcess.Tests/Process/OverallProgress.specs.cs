﻿using Machine.Fakes;
using Machine.Specifications;
using Rhino.Mocks;
using System.Threading;

namespace LongRunningProcess.Tests.Process
{
    [Subject(typeof(LongRunningProcess.Process))]
    public class When_marked_complete
    {
        Establish Context = () => Process = new LongRunningProcess.Process(string.Empty, null);

        Because Of = () => Process.Completed = true;

        It Should_be_at_100_percent = () => Process.OverallProgress.ShouldEqual(100);

        static IProcess Process;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_incremented
    {
        Establish Context = () => Process = new LongRunningProcess.Process(string.Empty, null);

        Because Of = () => Process.Report(50);

        It Should_be_at_50_percent = () => Process.OverallProgress.ShouldEqual(50);

        static IProcess Process;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_incremented_over_100
    {
        Establish Context = () => Process = new LongRunningProcess.Process(string.Empty, null);

        Because Of = () => Process.Report(101);

        It Should_be_capped_at_100_percent = () => Process.OverallProgress.ShouldEqual(100);

        static IProcess Process;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_there_is_one_child_with_progress : WithFakes
    {
        Establish Context = () =>
        {
            Factory = An<IProcessFactory>();

            Process = new LongRunningProcess.Process(string.Empty, Factory);

            Child = An<IProcess>();
            Child.WhenToldTo(c => c.OverallProgress).Return(50);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Factory.WhenToldTo(c => c.Create(Param<string>.IsAnything, Param<CancellationTokenSource>.IsAnything)).Return(Child);
        };

        Because Of = () => Process.Step(string.Empty, 50);

        It Should_compute_the_correct_percentage = () => Process.OverallProgress.ShouldEqual(25);

        static IProcessFactory Factory;

        static IProcess Process;

        static IProcess Child;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_there_are_multiple_children : WithFakes
    {
        Establish Context = () =>
        {
            Factory = MockRepository.GenerateStub<IProcessFactory>();

            Process = new LongRunningProcess.Process(string.Empty, Factory);

            Child = An<IProcess>();
            Child.WhenToldTo(c => c.OverallProgress).Return(10);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Child2 = An<IProcess>();
            Child2.WhenToldTo(c => c.OverallProgress).Return(20);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child).Repeat.Once();
            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child2).Repeat.Once();
        };

        Because Of = () =>
        {
            Process.Step(string.Empty, 40);
            Process.Step(string.Empty, 50);
        };

        It Should_combine_two_child_percentages = () => Process.OverallProgress.ShouldEqual(14);

        static IProcessFactory Factory;

        static IProcess Process;

        static IProcess Child;

        static IProcess Child2;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_there_are_multiple_overflow_children : WithFakes
    {
        Establish Context = () =>
        {
            Factory = MockRepository.GenerateStub<IProcessFactory>();

            Process = new LongRunningProcess.Process(string.Empty, Factory);

            Child = An<IProcess>();
            Child.WhenToldTo(c => c.OverallProgress).Return(99);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Child2 = An<IProcess>();
            Child2.WhenToldTo(c => c.OverallProgress).Return(98);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child).Repeat.Once();
            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child2).Repeat.Once();
        };

        Because Of = () =>
        {
            Process.Step(string.Empty, 60);
            Process.Step(string.Empty, 50);
        };

        It Should_cap_the_percentage_complete = () => Process.OverallProgress.ShouldEqual(100);

        static IProcessFactory Factory;

        static IProcess Process;

        static IProcess Child;

        static IProcess Child2;
    }

    [Subject(typeof(LongRunningProcess.Process))]
    public class When_there_are_multiple_children_and_increment : WithFakes
    {
        Establish Context = () =>
        {
            Factory = MockRepository.GenerateStub<IProcessFactory>();

            Process = new LongRunningProcess.Process(string.Empty, Factory);

            Child = An<IProcess>();
            Child.WhenToldTo(c => c.OverallProgress).Return(10);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Child2 = An<IProcess>();
            Child2.WhenToldTo(c => c.OverallProgress).Return(20);
            Child.WhenToldTo(c => c.OverallStatus).Return(new[] { string.Empty });

            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child).Repeat.Once();
            Factory.Stub(f => f.Create(Arg<string>.Is.Anything, Arg<CancellationTokenSource>.Is.Anything)).Return(Child2).Repeat.Once();
        };

        Because Of = () =>
        {
            Process.Step(string.Empty, 40);
            Process.Step(string.Empty, 50);
            Process.Report(50);
        };

        It Should_combine_two_child_percentages = () => Process.OverallProgress.ShouldEqual(19);

        static IProcessFactory Factory;

        static IProcess Process;

        static IProcess Child;

        static IProcess Child2;
    }
}
