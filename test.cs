public void Calculate(List<double> prices, List<DateTime> times)
{
    IsValid = false;

    if (prices == null || times == null)
        throw new ArgumentNullException(nameof(prices) + " 或 " + nameof(times));

    int n = prices.Count;
    if (times.Count != n)
        throw new ArgumentException("prices 和 times 的长度不匹配");

    if (n == 0)
    {
        Slope = Intercept = 0;
        StartPoint = EndPoint = default;
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

    // 验证价格和时间
    for (int i = 0; i < n; i++)
    {
        if (double.IsNaN(prices[i]) || double.IsInfinity(prices[i]))
        {
            Slope = Intercept = double.NaN;
            StartPoint = EndPoint = default;
            return;
        }
        if (times[i] == DateTime.MinValue || times[i] == DateTime.MaxValue)
            throw new ArgumentException("times 包含无效的 DateTime 值");
    }

    // 验证时间升序
    for (int i = 1; i < n; i++)
        if (times[i] < times[i - 1])
            throw new ArgumentException("times 必须按升序排列");

    // 使用 TotalMilliseconds 处理短时间跨度
    double[] x = new double[n];
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
        double sum = prices.Sum();
        Intercept = sum / n;
        StartPoint = new PricePoint(times[0], Intercept);
        EndPoint = StartPoint;
        IsValid = true;
        return;
    }

    double sumX = 0, sumY = 0, sumXY = 0, sumXX = 0;
    for (int i = 0; i < n; i++)
    {
        x[i] = (times[i] - baseTime).TotalMilliseconds; // 使用毫秒
        sumX += x[i];
        sumY += prices[i];
        sumXY += x[i] * prices[i];
        sumXX += x[i] * x[i];
    }

    double denominator = n * sumXX - sumX * sumX;
    if (Math.Abs(denominator) < 1e-10 * Math.Max(1.0, Math.Abs(sumXX)))
    {
        Slope = Intercept = double.NaN;
        StartPoint = EndPoint = default;
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
