public class MlPredictionType
{
    public string Family { get; set; }
    public string Type { get; set; }
    public double Confidence { get; set; }
    public Prediction[] AllPredictions { get; set; }
}

public class Prediction
{
    public string Family { get; set; }
    public string Type { get; set; }
    public double Confidence { get; set; }
}