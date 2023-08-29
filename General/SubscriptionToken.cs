using System;

namespace UnityToolbox.General
{
    /// <summary>
    /// A script which can be used to create indentifiers.
    /// </summary>
    public readonly struct SubscriptionToken
    {
        private readonly Guid _guid;

        public SubscriptionToken(Guid? templateGuid = null)
        {
            _guid = templateGuid ?? Guid.NewGuid();
        }

        public bool Equals(SubscriptionToken other) => _guid.Equals(other._guid);

        public override bool Equals(object obj) => obj is SubscriptionToken other && Equals(other);

        public override int GetHashCode() => _guid.GetHashCode();
    }
}
