public void Calculate(List<double> prices, List<DateTime> times)
{
    IsValid = false;

    if (prices == null || times == null)
    {
        throw new ArgumentNullException("prices or times");
    }

    int n = prices.Count;
    if (times.Count != n)
    {
        throw new ArgumentException("Length mismatch");
    }

    if (n == 0)
    {
        Slope = 0;
        Intercept = 0;
        StartPoint = default(PricePoint);
        EndPoint = default(PricePoint);
        return;
    }

    if (n == 1)
    {
        Slope = 0;
        Intercept = prices[0];
        StartPoint = new PricePoint(times[0], prices[0]);
        EndPoint = StartPoint;
        IsValid = true;
        return;
    }

    for (int i = 0; i < n; i++)
    {
        if (double.IsNaN(prices[i]) || double.IsInfinity(prices[i]))
        {
            Slope = double.NaN;
            Intercept = double.NaN;
            StartPoint = default(PricePoint);
            EndPoint = default(PricePoint);
            return;
        }
    }

    double[] x = new double[n];
    double[] y = prices.ToArray();
    DateTime baseTime = times[0];
    bool allTimesEqual = true;
    for (int i = 1; i < n; i++)
    {
        if (times[i] != baseTime)
        {
            allTimesEqual = false;
            break;
        }
    }
    if (allTimesEqual)
    {
        Slope = 0;
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            sum += prices[i];
        }
        Intercept = sum / n;
        StartPoint = new PricePoint(times[0], Intercept);
        EndPoint = StartPoint;
        IsValid = true;
        return;
    }

    double sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;
    for (int i = 0; i < n; i++)
    {
        x[i] = (times[i] - baseTime).TotalSeconds;
        y[i] = prices[i];
        sumX += x[i];
        sumY += y[i];
        sumXY += x[i] * y[i];
        sumXX += x[i] * x[i];
    }

    double denominator = n * sumXX - sumX * sumX;
    if (Math.Abs(denominator) < 1e-9)
    {
        Slope = double.NaN;
        Intercept = double.NaN;
        StartPoint = default(PricePoint);
        EndPoint = default(PricePoint);
        return;
    }

    Slope = (n * sumXY - sumX * sumY) / denominator;
    Intercept = (sumY - Slope * sumX) / n;

    double startPrice = Slope * x[0] + Intercept;
    double endPrice = Slope * x[n - 1] + Intercept;

    StartPoint = new PricePoint(times[0], startPrice);
    EndPoint = new PricePoint(times[n - 1], endPrice);
    IsValid = true;
}
