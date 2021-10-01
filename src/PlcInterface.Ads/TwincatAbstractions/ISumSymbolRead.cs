﻿using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads.SumCommand;

namespace PlcInterface.Ads.TwincatAbstractions
{
    /// <summary>
    /// A Abstraction layer over <see cref="SumSymbolRead"/>.
    /// </summary>
    public interface ISumSymbolRead
    {
        /// <inheritdoc cref="SumSymbolRead.Read" />
        public object[] Read();

        /// <inheritdoc cref="SumSymbolRead.ReadAsync(CancellationToken)" />
        Task<object[]?> ReadAsync(CancellationToken cancel);
    }
}