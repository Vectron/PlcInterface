﻿using System;

namespace TwinCAT.Ads;

/// <summary>
/// Extension methods for <see cref="AdsClient"/>.
/// </summary>
internal static class TcAdsClientExtension
{
    /// <summary>
    /// Validate the PLC connection.
    /// </summary>
    /// <param name="client">The <see cref="AdsClient"/> to check.</param>
    /// <returns>The <see cref="AdsClient"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">When the plc is not in a vallid state.</exception>
    public static IAdsConnection ValidateConnection(this IAdsConnection client)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        if (!client.IsConnected)
        {
            throw new InvalidOperationException("PLC not connected");
        }

        var errorCode = client.TryReadState(out var lastPLCState);

        if (errorCode != AdsErrorCode.NoError)
        {
            throw new InvalidOperationException("Unable to read the PLC state");
        }

        if (lastPLCState.AdsState != AdsState.Run)
        {
            throw new InvalidOperationException("PLC not running");
        }

        return client;
    }
}