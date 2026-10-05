using GelatinCapsule.Application.Features.Melting.DTOs;

namespace GelatinCapsule.Application.Features.Melting.Services;

/// <summary>
/// محاسبات مجوز ساخت - عیناً از VBA اکسس
/// </summary>
public static class ReleasePermissionCalculator
{
    private const double SLOPE = 0.08152;
    private const double INTERCEPT = 0.437019;
    private const double DENSITY = 1.096;

    public static CalculationResult Calculate(
        string part,        // "melt" / "pst"
        decimal vol0,
        decimal vis0,
        decimal vis1,
        int lagTime)
    {
        var result = new CalculationResult();

        // ====== 1) ویسکوزیته نهایی موثر ======
        // vis1m = vis1 + (lagtime * 5)
        var vis1m = vis1 + (lagTime * 5);

        // ====== 2) غلظت اولیه ======
        // c0 = Int((((0.4342945 * Log(vis0)) - intercept) / slope) * 100 + 0.5) / 100
        var c0Raw = ((0.4342945 * Math.Log((double)vis0)) - INTERCEPT) / SLOPE;
        var c0 = Math.Floor(c0Raw * 100 + 0.5) / 100;
        result.C0 = (decimal)c0;

        // ====== 3) غلظت نهایی ======
        var c1Raw = ((0.4342945 * Math.Log((double)vis1m)) - INTERCEPT) / SLOPE;
        var c1 = Math.Floor(c1Raw * 100 + 0.5) / 100;
        result.C1 = (decimal)c1;

        // ====== 4) حجم نهایی ======
        // vol1 = c0 * vol0 / c1
        var vol1 = c0 * (double)vol0 / c1;
        result.Vol1 = (decimal)vol1;

        // ====== 5) وزن اولیه ======
        // wei0 = vol0 * 1.096 * 1000
        var wei0 = (double)vol0 * DENSITY * 1000;
        result.Wei0 = (decimal)wei0;

        // ====== 6) محاسبات مخصوص هر محصول ======
        if (part == "melt")
        {
            // eqccapsule = Int((c0 / 100 * wei0) + 0.5)
            var eqc = Math.Floor((c0 / 100 * wei0) + 0.5);
            result.EqcCapsule = (decimal)eqc;

            // wtrneed = Int((vol0 * 1.096) - (eqccapsule / 1000))
            var wtr = Math.Floor(((double)vol0 * DENSITY) - (eqc / 1000));
            result.WtrNeed = (decimal)wtr;
        }
        else if (part == "pst")
        {
            // eqccapsule = Int(((vol0 * 1.096 * c0 / 100 * (20 / 100) + (vol0 * (20 / 100)))) * 1000 + 0.5)
            var eqcRaw = ((double)vol0 * DENSITY * c0 / 100 * (20.0 / 100))
                       + ((double)vol0 * (20.0 / 100));
            var eqc = Math.Floor(eqcRaw * 1000 + 0.5);
            result.EqcCapsule = (decimal)eqc;

            // wtrneed = Int(((vol0 * 1.096 * (60 / 100))) * 1000 + 0.5)
            var wtrRaw = ((double)vol0 * DENSITY * (60.0 / 100)) * 1000;
            var wtr = Math.Floor(wtrRaw + 0.5);
            result.WtrNeed = (decimal)wtr;
        }

        return result;
    }
}