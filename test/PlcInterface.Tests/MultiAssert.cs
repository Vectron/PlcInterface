using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlcInterface.Tests
{
    public class MultiAssert
    {
        private readonly List<AssertFailedException> exceptions = new();

        public static void Aggregate(params Action[] actions)
        {
            var multiAssert = new MultiAssert();

            foreach (var action in actions)
            {
                multiAssert.Check(action);
            }

            multiAssert.Assert();
        }

        public void Assert()
        {
            var assertionTexts = exceptions.Select(assertFailedException => assertFailedException.Message);
            if (assertionTexts.Any())
            {
                throw new AssertFailedException(assertionTexts.Aggregate((aggregatedMessage, next) => aggregatedMessage + Environment.NewLine + next));
            }
        }

        public void Check(Action action)
        {
            try
            {
                action();
            }
            catch (AssertFailedException ex)
            {
                exceptions.Add(ex);
            }
        }
    }
}