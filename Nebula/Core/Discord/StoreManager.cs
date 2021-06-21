using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Nebula.Core.Discord
{
    public partial class StoreManager
    {
        public IEnumerable<Entitlement> GetEntitlements()
        {
            int count = CountEntitlements();
            var entitlements = new List<Entitlement>();
            for (var i = 0; i < count; i++) entitlements.Add(GetEntitlementAt(i));

            return entitlements;
        }

        public IEnumerable<Sku> GetSkus()
        {
            int count = CountSkus();
            var skus = new List<Sku>();
            for (var i = 0; i < count; i++) skus.Add(GetSkuAt(i));

            return skus;
        }
    }
}