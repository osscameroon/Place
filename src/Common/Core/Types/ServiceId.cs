using System;

namespace Core.Types;

/// <summary>
/// Default implementation of IServiceId using GUID.
/// </summary>
internal class ServiceId : IServiceId
{
    /// <inheritdoc />
    public string Id { get; } = $"{Guid.NewGuid():N}";
}
