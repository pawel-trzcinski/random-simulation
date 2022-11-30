using System;
using System.Collections.Generic;

namespace RandomSimulationEngine.Tasks;

public class BytesProvidingResult
{
    public bool IsDtataAvailable { get; }
    public IReadOnlyCollection<byte> Data { get; }

    private BytesProvidingResult(IReadOnlyCollection<byte> data)
    {
        Data = data;
        IsDtataAvailable = data.Count > 0;
    }

    public static BytesProvidingResult Empty()
    {
        return new BytesProvidingResult(Array.Empty<byte>());
    }

    public static BytesProvidingResult Create(byte[] data)
    {
        return new BytesProvidingResult(data);
    }
}