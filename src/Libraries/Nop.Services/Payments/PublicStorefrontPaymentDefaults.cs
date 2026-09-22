namespace Nop.Services.Payments;

/// <summary>
/// Public storefront payment-method defaults for this store.
/// Manual credit card entry is not offered (raw PAN/CVV must not be collected).
/// PayPal Commerce stays inactive until merchant credentials are configured.
/// Check/Money Order remains available for offline B2B.
/// </summary>
public static partial class PublicStorefrontPaymentDefaults
{
    /// <summary>
    /// Gets the Check / Money Order payment method system name
    /// </summary>
    public static string CheckMoneyOrderSystemName => "Payments.CheckMoneyOrder";

    /// <summary>
    /// Gets the Manual (credit card stored in database) payment method system name
    /// </summary>
    public static string ManualSystemName => "Payments.Manual";

    /// <summary>
    /// Gets the PayPal Commerce payment method system name
    /// </summary>
    public static string PayPalCommerceSystemName => "Payments.PayPalCommerce";

    /// <summary>
    /// Gets system names of payment methods that should be active on the public storefront
    /// </summary>
    public static IReadOnlyList<string> ActivePaymentMethodSystemNames { get; } =
        [CheckMoneyOrderSystemName];

    /// <summary>
    /// Removes Manual and PayPal Commerce from the active payment method list and
    /// ensures Check/Money Order remains available.
    /// </summary>
    /// <param name="activePaymentMethodSystemNames">Current active payment method system names</param>
    /// <returns>Payment method system names allowed on the public storefront</returns>
    public static List<string> ForPublicStorefront(IEnumerable<string> activePaymentMethodSystemNames)
    {
        var result = (activePaymentMethodSystemNames ?? [])
            .Where(name =>
                !string.Equals(name, ManualSystemName, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(name, PayPalCommerceSystemName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!result.Exists(name => string.Equals(name, CheckMoneyOrderSystemName, StringComparison.OrdinalIgnoreCase)))
            result.Add(CheckMoneyOrderSystemName);

        return result;
    }
}
